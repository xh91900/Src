using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SuperProducer.Core.Utility.Encrypt
{
    public class RSA : EncryptBase
    {
        private enum KeyType
        {
            Path = 1,
            Content
        }

        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt(string str, string public_key)
        {
            string retVal = string.Empty;
            try
            {
                public_key = public_key.Trim('/');

                var currentKeyType = KeyType.Content;
                var currentKeyContent = public_key;

                if (RegExpHelper.IsUrl(public_key) || RegExpHelper.IsRelativePath(public_key) || RegExpHelper.IsPhysicalPath(public_key))
                    currentKeyType = KeyType.Path;

                if (currentKeyType == KeyType.Path)
                {
                    currentKeyContent = FileHelper.GetFileContent(public_key);
                }

                if (!string.IsNullOrEmpty(currentKeyContent))
                {
                    var rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    LoadPublicKeyPEM(rsa, currentKeyContent);

                    byte[] dataBuffer = this.DefaultEncode.GetBytes(str);
                    int maxBlockSize = rsa.KeySize / 8 - 11; //加密块最大长度限制
                    if (dataBuffer.Length > maxBlockSize)
                    {
                        MemoryStream plaiStream = new MemoryStream(dataBuffer);
                        MemoryStream crypStream = new MemoryStream();
                        Byte[] buffer = new Byte[maxBlockSize];
                        int blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                        while (blockSize > 0)
                        {
                            Byte[] toEncrypt = new Byte[blockSize];
                            Array.Copy(buffer, 0, toEncrypt, 0, blockSize);
                            Byte[] cryptograph = rsa.Encrypt(toEncrypt, false);
                            crypStream.Write(cryptograph, 0, cryptograph.Length);
                            blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                        }
                        retVal = Convert.ToBase64String(crypStream.ToArray(), Base64FormattingOptions.None);
                    }
                    else
                    {
                        retVal = Convert.ToBase64String(rsa.Encrypt(dataBuffer, false));
                    }
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public string Decrypt(string str, string private_key)
        {
            string retVal = string.Empty;
            try
            {
                private_key = private_key.Trim('/');

                var currentKeyType = KeyType.Content;
                var currentKeyContent = private_key;

                if (RegExpHelper.IsUrl(private_key) || RegExpHelper.IsRelativePath(private_key) || RegExpHelper.IsPhysicalPath(private_key))
                    currentKeyType = KeyType.Path;

                if (currentKeyType == KeyType.Path)
                {
                    currentKeyContent = FileHelper.GetFileContent(private_key);
                    currentKeyContent = RemoveHeaderFooter("RSA PRIVATE KEY", currentKeyContent);
                }

                if (!string.IsNullOrEmpty(currentKeyContent))
                {
                    var rsaCsp = DecodeRSAPrivateKey(Convert.FromBase64String(currentKeyContent));
                    byte[] dataBuffer = Convert.FromBase64String(str);
                    int maxBlockSize = rsaCsp.KeySize / 8; //解密块最大长度限制
                    if (dataBuffer.Length > maxBlockSize)
                    {
                        MemoryStream crypStream = new MemoryStream(dataBuffer);
                        MemoryStream plaiStream = new MemoryStream();
                        Byte[] buffer = new Byte[maxBlockSize];
                        int blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                        while (blockSize > 0)
                        {
                            Byte[] toDecrypt = new Byte[blockSize];
                            Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                            Byte[] cryptograph = rsaCsp.Decrypt(toDecrypt, false);
                            plaiStream.Write(cryptograph, 0, cryptograph.Length);
                            blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                        }
                        retVal = this.DefaultEncode.GetString(plaiStream.ToArray());
                    }
                    else
                    {
                        retVal = this.DefaultEncode.GetString(rsaCsp.Decrypt(dataBuffer, false));
                    }
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign(string str, string private_key)
        {
            string retVal = string.Empty;
            try
            {
                private_key = private_key.Trim('/');

                var currentKeyType = KeyType.Content;
                var currentKeyContent = private_key;

                if (RegExpHelper.IsUrl(private_key) || RegExpHelper.IsRelativePath(private_key) || RegExpHelper.IsPhysicalPath(private_key))
                    currentKeyType = KeyType.Path;

                if (currentKeyType == KeyType.Path)
                {
                    currentKeyContent = FileHelper.GetFileContent(private_key);
                    currentKeyContent = RemoveHeaderFooter("RSA PRIVATE KEY", currentKeyContent);
                }

                if (!string.IsNullOrEmpty(currentKeyContent))
                {
                    var rsaCsp = DecodeRSAPrivateKey(Convert.FromBase64String(currentKeyContent));
                    byte[] signatureBytes = rsaCsp.SignData(this.DefaultEncode.GetBytes(str), new SHA1CryptoServiceProvider());
                    retVal = Convert.ToBase64String(signatureBytes);
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 校验
        /// </summary>
        public bool VerifyData(string sourceStr, string signatureStr, string public_key)
        {
            bool retVal = false;
            try
            {
                public_key = public_key.Trim('/');

                var currentKeyType = KeyType.Content;
                var currentKeyContent = public_key;

                if (RegExpHelper.IsUrl(public_key) || RegExpHelper.IsRelativePath(public_key) || RegExpHelper.IsPhysicalPath(public_key))
                    currentKeyType = KeyType.Path;

                if (currentKeyType == KeyType.Path)
                {
                    currentKeyContent = FileHelper.GetFileContent(public_key);
                }

                if (!string.IsNullOrEmpty(currentKeyContent))
                {
                    var rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    LoadPublicKeyPEM(rsa, currentKeyContent);
                    retVal = rsa.VerifyData(this.DefaultEncode.GetBytes(sourceStr), new SHA1CryptoServiceProvider(), Convert.FromBase64String(signatureStr));
                }
            }
            catch { }
            return retVal;
        }



        #region "辅助方法"

        #region Methods

        /// <summary>Extension method which initializes an RSACryptoServiceProvider from a DER public key blob.</summary>
        internal static void LoadPublicKeyDER(RSACryptoServiceProvider provider, byte[] DERData)
        {
            byte[] RSAData = GetRSAFromDER(DERData);
            byte[] publicKeyBlob = GetPublicKeyBlobFromRSA(RSAData);
            provider.ImportCspBlob(publicKeyBlob);
        }

        /// <summary>Extension method which initializes an RSACryptoServiceProvider from a PEM public key string.</summary>
        internal static void LoadPublicKeyPEM(RSACryptoServiceProvider provider, string sPEM)
        {
            byte[] DERData = GetDERFromPEM(sPEM);
            LoadPublicKeyDER(provider, DERData);
        }

        /// <summary>Returns a public key blob from an RSA public key.</summary>
        internal static byte[] GetPublicKeyBlobFromRSA(byte[] RSAData)
        {
            byte[] data = null;
            UInt32 dwCertPublicKeyBlobSize = 0;
            if (CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING,
                new IntPtr((int)CRYPT_OUTPUT_TYPES.RSA_CSP_PUBLICKEYBLOB), RSAData, (UInt32)RSAData.Length, CRYPT_DECODE_FLAGS.NONE,
                data, ref dwCertPublicKeyBlobSize))
            {
                data = new byte[dwCertPublicKeyBlobSize];
                if (!CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING,
                    new IntPtr((int)CRYPT_OUTPUT_TYPES.RSA_CSP_PUBLICKEYBLOB), RSAData, (UInt32)RSAData.Length, CRYPT_DECODE_FLAGS.NONE,
                    data, ref dwCertPublicKeyBlobSize))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return data;
        }

        /// <summary>Converts DER binary format to a CAPI CERT_PUBLIC_KEY_INFO structure containing an RSA key.</summary>
        internal static byte[] GetRSAFromDER(byte[] DERData)
        {
            byte[] data = null;
            byte[] publicKey = null;
            CERT_PUBLIC_KEY_INFO info;
            UInt32 dwCertPublicKeyInfoSize = 0;
            IntPtr pCertPublicKeyInfo = IntPtr.Zero;
            if (CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.X509_PUBLIC_KEY_INFO),
                DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwCertPublicKeyInfoSize))
            {
                data = new byte[dwCertPublicKeyInfoSize];
                if (CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.X509_PUBLIC_KEY_INFO),
                    DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwCertPublicKeyInfoSize))
                {
                    GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    try
                    {
                        info = (CERT_PUBLIC_KEY_INFO)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CERT_PUBLIC_KEY_INFO));
                        publicKey = new byte[info.PublicKey.cbData];
                        Marshal.Copy(info.PublicKey.pbData, publicKey, 0, publicKey.Length);
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return publicKey;
        }

        /// <summary>Extracts the binary data from a PEM file.</summary>
        internal static byte[] GetDERFromPEM(string sPEM)
        {
            UInt32 dwSkip, dwFlags;
            UInt32 dwBinarySize = 0;

            if (!CryptStringToBinary(sPEM, (UInt32)sPEM.Length, CRYPT_STRING_FLAGS.CRYPT_STRING_BASE64HEADER, null, ref dwBinarySize, out dwSkip, out dwFlags))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            byte[] decodedData = new byte[dwBinarySize];
            if (!CryptStringToBinary(sPEM, (UInt32)sPEM.Length, CRYPT_STRING_FLAGS.CRYPT_STRING_BASE64HEADER, decodedData, ref dwBinarySize, out dwSkip, out dwFlags))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return decodedData;
        }

        #endregion Methods

        #region P/Invoke Constants

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_ACQUIRE_CONTEXT_FLAGS : uint
        {
            CRYPT_NEWKEYSET = 0x8,
            CRYPT_DELETEKEYSET = 0x10,
            CRYPT_MACHINE_KEYSET = 0x20,
            CRYPT_SILENT = 0x40,
            CRYPT_DEFAULT_CONTAINER_OPTIONAL = 0x80,
            CRYPT_VERIFYCONTEXT = 0xF0000000
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_PROVIDER_TYPE : uint
        {
            PROV_RSA_FULL = 1
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_DECODE_FLAGS : uint
        {
            NONE = 0,
            CRYPT_DECODE_ALLOC_FLAG = 0x8000
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_ENCODING_FLAGS : uint
        {
            PKCS_7_ASN_ENCODING = 0x00010000,
            X509_ASN_ENCODING = 0x00000001,
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_OUTPUT_TYPES : int
        {
            X509_PUBLIC_KEY_INFO = 8,
            RSA_CSP_PUBLICKEYBLOB = 19,
            PKCS_RSA_PRIVATE_KEY = 43,
            PKCS_PRIVATE_KEY_INFO = 44
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_STRING_FLAGS : uint
        {
            CRYPT_STRING_BASE64HEADER = 0,
            CRYPT_STRING_BASE64 = 1,
            CRYPT_STRING_BINARY = 2,
            CRYPT_STRING_BASE64REQUESTHEADER = 3,
            CRYPT_STRING_HEX = 4,
            CRYPT_STRING_HEXASCII = 5,
            CRYPT_STRING_BASE64_ANY = 6,
            CRYPT_STRING_ANY = 7,
            CRYPT_STRING_HEX_ANY = 8,
            CRYPT_STRING_BASE64X509CRLHEADER = 9,
            CRYPT_STRING_HEXADDR = 10,
            CRYPT_STRING_HEXASCIIADDR = 11,
            CRYPT_STRING_HEXRAW = 12,
            CRYPT_STRING_NOCRLF = 0x40000000,
            CRYPT_STRING_NOCR = 0x80000000
        }

        #endregion P/Invoke Constants

        #region P/Invoke Structures

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_OBJID_BLOB
        {
            internal UInt32 cbData;
            internal IntPtr pbData;
        }

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_ALGORITHM_IDENTIFIER
        {
            internal IntPtr pszObjId;
            internal CRYPT_OBJID_BLOB Parameters;
        }

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        struct CRYPT_BIT_BLOB
        {
            internal UInt32 cbData;
            internal IntPtr pbData;
            internal UInt32 cUnusedBits;
        }

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        struct CERT_PUBLIC_KEY_INFO
        {
            internal CRYPT_ALGORITHM_IDENTIFIER Algorithm;
            internal CRYPT_BIT_BLOB PublicKey;
        }

        #endregion P/Invoke Structures

        #region P/Invoke Functions

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptDestroyKey(IntPtr hKey);

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptImportKey(IntPtr hProv, byte[] pbKeyData, UInt32 dwDataLen, IntPtr hPubKey, UInt32 dwFlags, ref IntPtr hKey);

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptReleaseContext(IntPtr hProv, Int32 dwFlags);

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptAcquireContext(ref IntPtr hProv, string pszContainer, string pszProvider, CRYPT_PROVIDER_TYPE dwProvType, CRYPT_ACQUIRE_CONTEXT_FLAGS dwFlags);

        /// <summary>Function from Crypto API.</summary>
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptStringToBinary(string sPEM, UInt32 sPEMLength, CRYPT_STRING_FLAGS dwFlags, [Out] byte[] pbBinary, ref UInt32 pcbBinary, out UInt32 pdwSkip, out UInt32 pdwFlags);

        /// <summary>Function from Crypto API.</summary>
        [DllImport("crypt32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptDecodeObjectEx(CRYPT_ENCODING_FLAGS dwCertEncodingType, IntPtr lpszStructType, byte[] pbEncoded, UInt32 cbEncoded, CRYPT_DECODE_FLAGS dwFlags, IntPtr pDecodePara, ref byte[] pvStructInfo, ref UInt32 pcbStructInfo);

        /// <summary>Function from Crypto API.</summary>
        [DllImport("crypt32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptDecodeObject(CRYPT_ENCODING_FLAGS dwCertEncodingType, IntPtr lpszStructType, byte[] pbEncoded, UInt32 cbEncoded, CRYPT_DECODE_FLAGS flags, [In, Out] byte[] pvStructInfo, ref UInt32 cbStructInfo);

        #endregion P/Invoke Functions


        private static string RemoveHeaderFooter(string flagText, string dataString)
        {
            try
            {
                string header = string.Format("-----BEGIN {0}-----\\n", flagText);
                string footer = string.Format("-----END {0}-----", flagText);
                int start = dataString.IndexOf(header) + header.Length;
                int end = dataString.IndexOf(footer, start);
                return dataString.Substring(start, (end - start));
            }
            catch { }
            return string.Empty;
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        #endregion
    }
}
