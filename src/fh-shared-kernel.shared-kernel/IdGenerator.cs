using IdGen;
using Microsoft.Extensions.Configuration;

namespace FamilyHubs.SharedKernel
{
    /// <summary>
    /// A 64 bit (BigInt/Long) sequential Id generator using IdGen by RobThree
    /// https://github.com/RobThree/IdGen/blob/master/IdGen/IdGenerator.cs
    /// 
    /// The 64 bits are composed of a timestamp, a generator id, and a sequence number as shown below.
    ///    |----------------------------------------------------------- timestamp -------------------------------------------------------------------|gen id|--------- sequence number --------|
    /// 64 63 62 61 60 59 58 57 56 55 54 53 52 51 50 49 48 47 46 45 44 43 42 41 40 39 38 37 36 35 34 33 32 31 30 29 28 27 26 25 24 23 22 21 20 19 18 17 16 15 14 13 12 11 10 9 8 7 6 5 4 3 2 1 0
    /// 
    /// 64th Bit.
    /// To avoid issues with the 64 bits being signed or unsigned the 64th bit is unused.
    /// 
    /// Timestamp.
    /// The timestamp is based on the .net 'Tick' and is an offset from a given Epoch (starting date) which on Unix systems is typically 1/1/1970, on our system it is 1/1/2020.
    /// 
    /// GeneratorId.
    /// The generator id is an arbitary number to help with uniqueness when using multiple Id generators. With the 'tick' set at 1 millisecond it's possible to generate the same Id number across multiple instances of the IdGenrator if the calls are within 1 millisecond.
    /// The generator id is a unque number used per generator to ensure Id's generated within the same millisecond do not generate the same number (effectively just the timestamp).
    /// 
    /// For our usage we use a unique generator id per instance of a generator, e.g. 
    ///     0 = IdAM service
    ///     1 = Service Directory Service
    ///     2 = Referral Service etc
    /// 
    /// Sequence Number.
    /// The sequence number is also a number to help with uniqueness. IdGen is stateful (it will need to be setup as a singleton) and stores each timestamp and sequence number it generates (so each new timestamp overwrites the previous timestamp) and increments the sequence number on each call to create a new id within a given timestamp period.
    /// e.g. if a Tick was every millisecond, IdGen will store the timestamp and then for each call to get an Id in that millisecond it will increment the sequence number.
    /// 
    /// For the Family Hubs portfolio of services we are using a configuration of 45 bits for the timestamp, 3 bits for the generator id and 15 bits for the sequence number and an Epoch (start point in time) of 1/1/2020.
    /// 
    /// App settings configuration format is:
    /// "IdGen64": 
    ///       "GeneratorId": "0"
    /// </summary>
    public class IdGen64
    {
        private readonly IdGenerator _idGenerator;

        public IdGen64()
        {
            _idGenerator = CreateIdGenerator();
        }

        public long NewId()
        {
            ArgumentNullException.ThrowIfNull(_idGenerator);

            return _idGenerator.CreateId();
        }

        public IdGenerator CreateIdGenerator()
        {
            // Get the generator configuration
            var idGen64Config = GetConfig();

            // Prepare the options
            var defaultTimeSource = new DefaultTimeSource(idGen64Config.Epoch);
            var options = new IdGeneratorOptions(idGen64Config.Structure, defaultTimeSource);
            var generator = new IdGenerator(idGen64Config.GeneratorId, options);

            return generator;
        }

        public sealed record IdGen64Config
        {
            public byte GeneratorId { get; set; }

            public DateTime Epoch { get; set; }

            public IdStructure? Structure { get; set; }
        }

        public static IdGen64Config GetConfig()
        {
            var idGen64Config = new IdGen64Config();

            // For our application the structure is fixed at 45 bits for the timestamp, 3 bits for the generator id and 15 bits for the sequence number
            idGen64Config.Structure = new IdStructure(45, 3, 15);

            // For our application the Epoch is fixed at 1/1/2020
            idGen64Config.Epoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var generatorIdSetting = GetGeneratorIdConfigSetting();

            idGen64Config.GeneratorId = 0;
            if (!string.IsNullOrEmpty(generatorIdSetting))
            {
                idGen64Config.GeneratorId = Convert.ToByte(generatorIdSetting);
            }

            return idGen64Config;
        }

        public static string GetGeneratorIdConfigSetting()
        {
            // Load the configuration file.
            IConfigurationRoot configurationBuilder;
            configurationBuilder = new ConfigurationBuilder().
               AddJsonFile("appsettings.json").Build();

            // Get the section to read
            var configSection = configurationBuilder.GetSection("IdGen64");

            // Get the generator id
            var generatorIdSetting = configSection["GeneratorId"];
            ArgumentNullException.ThrowIfNullOrEmpty(generatorIdSetting);

            return generatorIdSetting;
        }
    }
}
