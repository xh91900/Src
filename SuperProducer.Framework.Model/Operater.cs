﻿using System;

namespace SuperProducer.Framework.Model
{
    public class Operater
    {
        public Operater()
        {
            this.Name = "Anonymous";
        }

        public string Name { get; set; }

        public string IP { get; set; }

        public DateTime Time { get; set; }

        public Guid Token { get; set; }

        public int UserId { get; set; }

        public string Method { get; set; }
    }
}
