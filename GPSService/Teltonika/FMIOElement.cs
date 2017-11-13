namespace GPSService.Teltonika
{
    class FMIOElement
    {
        public int ID { get; set; }
        public int Valor { get; set; }

        public static FMIOElement Parse(byte[] message, int offset, int numBytesDatos)
        {
            FMIOElement item = new FMIOElement()
            {
                ID = ToInt(message, offset, 1),
                Valor = ToInt(message, offset + 1, numBytesDatos)
            };

            return item;
        }

        public override string ToString()
        {
            return $"{ID}={Valor}";
        }

        private static int ToInt(byte[] message, int offset, int len)
        {
            int valor = 0;
            for (int i = 0; i < len; i++)
            {
                valor = valor << 8;
                valor = valor | message[offset + i];
            }

            return valor;
        }
    }
}