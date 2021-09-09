namespace TableTopCrucible.Data.Library.Models.Values
{
    public struct Description
    {
        private string _description { get; }
        public Description(string name)
        {
            _description = name;
        }
        public override string ToString()
            => _description;
        public override bool Equals(object obj)
        {
            return obj switch
            {
                string str => _description == str,
                Description name => _description == name._description,
                _ => false,
            };
        }
        public override int GetHashCode()
            => _description.GetHashCode();
        public static explicit operator Description(string text)
            => new Description(text);
        public static explicit operator string(Description name)
            => name._description;

    }
}
