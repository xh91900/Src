using SuperProducer.Core.Utility;
using SuperProducer.Framework.Model;
using SuperProducer.Framework.Model.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SuperProducer.Framework.Web
{
    public class WebApiController : ApiController
    {
        private IDictionary<string, object> _CurrentFormParameters;

        /// <summary>
        /// 当前请求的所有Form参数[Action无参数时]
        /// </summary>
        protected IDictionary<string, object> CurrentFormParameters
        {
            get
            {
                if (this._CurrentFormParameters == null)
                {
                    this._CurrentFormParameters = new Dictionary<string, object>(this.ActionContext.ActionArguments);
                }
                return _CurrentFormParameters;
            }
        }

        /// <summary>
        /// 根据Key获取当前请求的Form参数[Action无参数时]
        /// </summary>
        protected T GetFormParameter<T>(string key, T defaultValue = default(T))
        {
            if (this.ActionContext.ActionDescriptor.GetParameters().Count == 0)
            {
                if (this.ActionContext.ActionArguments.ContainsKey(key))
                {
                    try
                    {

                        return (T)ConvertHelper.ChangeType<T>(this.ActionContext.ActionArguments[key]);
                    }
                    catch { }
                }

            }
            return defaultValue;
        }

        /// <summary>
        /// 获取第一个模型验证错误
        /// </summary>
        protected ModelStateError GetFirstModelError()
        {
            ModelStateError retVal = null;

            if (this.CurrentFormParameters != null && this.CurrentFormParameters.Count == 1 && this.CurrentFormParameters.First().Key == "model") // 硬编码
            {
                if (this.CurrentFormParameters.FirstOrDefault().Value == null)
                {
                    retVal = new ModelStateError()
                    {
                        key = this.CurrentFormParameters.First().Key,
                        msg = ConvertHelper.GetString((int)CommonEnum.ProgErrorString.Key_999996)
                    };
                }
                else if (!this.ModelState.IsValid)
                {
                    try
                    {
                        #region "Sort"

                        var allPropertysForErrors = new Dictionary<string, int>();

                        var allPropertys = ObjectHelper.GetProperties(this.CurrentFormParameters.First().Value);
                        foreach (var item in ModelState.Keys)
                        {
                            var index = 0;
                            for (int i = 0; i < allPropertys.Length; i++)
                            {
                                if (allPropertys[i].Name == (item.Contains(".") ? item.Substring(item.LastIndexOf(".") + 1) : item))
                                {
                                    index = i;
                                    break;
                                }
                            }
                            allPropertysForErrors.Add(item, index);
                        }

                        allPropertysForErrors = allPropertysForErrors.OrderBy(item => item.Value).ToDictionary(k => k.Key, v => v.Value);

                        #endregion

                        foreach (var item in allPropertysForErrors)
                        {
                            var error = this.ModelState[item.Key].Errors.Where(value => RegExpHelper.IsAllNumber(value.ErrorMessage)).FirstOrDefault();
                            if (error != null)
                            {
                                retVal = new ModelStateError()
                                {
                                    key = (item.Key.Contains(".") ? item.Key.Substring(item.Key.LastIndexOf(".") + 1) : item.Key),
                                    msg = error.ErrorMessage
                                };
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }
            return retVal;
        }

        /// <summary>
        /// 增加参数到ResultModel的格式化参数中
        /// </summary>
        protected void AddFormatParasToIResultModel(IResultModel resultModel, ModelStateError firstModelError = null)
        {
            if (resultModel == null)
                return;

            var error = firstModelError == null ? this.GetFirstModelError() : firstModelError;
            if (error == null || string.IsNullOrEmpty(error.key))
                return;

            var errorProperty = this.CurrentFormParameters.First().Value.GetType().GetProperty(error.key);
            if (errorProperty == null)
                return;

            var validAttrs = errorProperty.GetCustomAttributes<ValidationAttribute>();
            if (validAttrs == null)
                return;

            var targetValidAttr = validAttrs.Where(item => item.ErrorMessage == error.msg).FirstOrDefault();
            if (targetValidAttr != null)
            {
                if (targetValidAttr is StringLengthAttribute stringAttr)
                {
                    AddFormatParasToIResultModel(resultModel, stringAttr.MinimumLength, stringAttr.MaximumLength);
                }
                else if (targetValidAttr is YearRangeValidityAttribute yearAttr)
                {
                    AddFormatParasToIResultModel(resultModel, yearAttr.MinDate.ToCnDateString(), yearAttr.MaxDate.ToCnDateString());
                }
                else if (targetValidAttr is RangeAttribute rangeAttr)
                {
                    AddFormatParasToIResultModel(resultModel, rangeAttr.Minimum, rangeAttr.Maximum);
                }
            }

            if (resultModel.code <= 0)
            {
                resultModel.code = ConvertHelper.GetInt(error.msg);
            }
        }

        /// <summary>
        /// 增加参数到ResultModel的格式化参数中
        /// </summary>
        protected void AddFormatParasToIResultModel(IResultModel resultModel, params object[] paras)
        {
            if (resultModel == null || paras == null || paras.Length == 0)
                return;

            resultModel.MsgFormatParameter.AddRange(paras);
        }



        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            var executeTask = base.ExecuteAsync(controllerContext, cancellationToken);

            if (controllerContext.Controller is ApiController)
            {
                var targteController = controllerContext.Controller as ApiController;
                var request = targteController.ActionContext.Request;
                var response = targteController.ActionContext.Response;

                if (response != null && response.Content != null)
                {
                    #region "IResultModel"

                    if (response.Content is ObjectContent content && typeof(IResultModel).IsAssignableFrom(content.ObjectType))
                    {
                        var taskInfo = content.ReadAsAsync<IResultModel>(); taskInfo.Wait(5000);
                        if (taskInfo.Status == TaskStatus.RanToCompletion && taskInfo.Result != null)
                        {
                            var resultModel = taskInfo.Result;

                            #region "模型格式化参数"

                            if (resultModel.code >= (int)CommonEnum.CodeRangeForModel.Start && resultModel.code <= (int)CommonEnum.CodeRangeForModel.End)
                            {
                                var error = GetFirstModelError();
                                if (error != null)
                                    this.AddFormatParasToIResultModel(resultModel, error);
                            }

                            #endregion

                            resultModel.Refresh();
                        }
                    }

                    #endregion
                }
            }

            return executeTask;
        }
    }
}
