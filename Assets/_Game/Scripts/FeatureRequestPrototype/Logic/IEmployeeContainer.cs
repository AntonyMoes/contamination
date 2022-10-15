namespace _Game.Scripts.FeatureRequestPrototype.Logic {
    public interface IEmployeeContainer {
        public int Position { get; }
        
        public Employee Employee { get; }

        public void SwapWith(IEmployeeContainer other);
    }
}