namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public class BaseData : CharacterData
    {
        public BaseConfig BaseConfig { get; private set; }
        public BaseData(BaseConfig baseConfig) : base(baseConfig.MoveSpeed)
        {
            BaseConfig = baseConfig;
        }
    }
}
