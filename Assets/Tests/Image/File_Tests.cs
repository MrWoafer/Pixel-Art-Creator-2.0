using NUnit.Framework;

using PAC.DataStructures;
using PAC.Image;
using PAC.Json;

namespace PAC.Tests
{
    /// <summary>
    /// Tests the File class.
    /// </summary>
    public class File_Tests
    {
        /// <summary>
        /// Checks that ToJson() and FromJson() run without exceptions for the custom JSON converter for type File.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void JsonNoExceptions()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new File.JsonConverter(Config.Files.fileFormatVersion));

            File file = new File("File", 16, 16);

            Assert.DoesNotThrow(() => JsonConversion.ToJson(file, converters, false));

            JsonData.Object json = (JsonData.Object)JsonConversion.ToJson(file, converters, false);
            json = json.Prepend(new JsonData.Object {
                { "file format version", JsonConversion.ToJson(Config.Files.fileFormatVersion, new JsonConversion.JsonConverterSet(new SemanticVersion.JsonConverter()), false) }
            });

            Assert.DoesNotThrow(() => JsonConversion.FromJson<File>(json, converters, false));
        }
    }
}
