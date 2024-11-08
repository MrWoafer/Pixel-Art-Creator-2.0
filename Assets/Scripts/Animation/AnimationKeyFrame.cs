using PAC.DataStructures;
using PAC.Extensions;
using PAC.Json;
using PAC.JSON;
using System.Runtime.Serialization;
using UnityEngine;

namespace PAC.Animation
{
    /// <summary>
    /// A class representing a single keyframe for a layer.
    /// </summary>
    public class AnimationKeyFrame
    {
        /// <summary>The number of the frame this keyframe is on.</summary>
        public int frame;
        /// <summary>The texture displayed at this keyframe.</summary>
        public Texture2D texture;

        public AnimationKeyFrame(int frame, Texture2D texture)
        {
            if (frame < 0)
            {
                throw new System.ArgumentException("Frame index cannot be negative: " + frame);
            }

            this.frame = frame;
            this.texture = texture;
        }

        /// <summary>
        /// Creates a deep copy of the AnimationKeyFrame.
        /// </summary>
        public AnimationKeyFrame(AnimationKeyFrame animationKeyFrame) : this(animationKeyFrame.frame, Tex2DSprite.Copy(animationKeyFrame.texture)) { }

        /// <summary>
        /// Creates a deep copy of the AnimationKeyFrame.
        /// </summary>
        public AnimationKeyFrame DeepCopy()
        {
            return new AnimationKeyFrame(this);
        }

        public class JsonConverter : JsonConversion.JsonConverter<AnimationKeyFrame, JsonObj>
        {
            private SemanticVersion fromJsonFileFormatVersion;

            public JsonConverter(SemanticVersion fromJsonFileFormatVersion)
            {
                this.fromJsonFileFormatVersion = fromJsonFileFormatVersion;
            }

            public override JsonObj ToJson(AnimationKeyFrame keyFrame)
            {
                return new JsonObj
                    {
                        { "frame", keyFrame.frame },
                        { "texture", JsonConversion.ToJson(keyFrame.texture,
                            new JsonConversion.JsonConverterSet(new JsonConverters.Texture2DJsonConverter()), false) }
                    };
            }

            public override AnimationKeyFrame FromJson(JsonObj jsonData)
            {
                if (fromJsonFileFormatVersion > Config.Files.fileFormatVersion)
                {
                    throw new SerializationException("The JSON uses file format version " + fromJsonFileFormatVersion + ", which is ahead of the current version " + Config.Files.fileFormatVersion);
                }
                if (fromJsonFileFormatVersion.major < Config.Files.fileFormatVersion.major)
                {
                    throw new SerializationException("The JSON uses file format version " + fromJsonFileFormatVersion + ", which is out of date with the current version " + Config.Files.fileFormatVersion);
                }

                int frame = JsonConversion.FromJson<int>(jsonData["frame"]);
                Texture2D tex = JsonConversion.FromJson<Texture2D>(jsonData["texture"], new JsonConversion.JsonConverterSet(new JsonConverters.Texture2DJsonConverter()), false);

                return new AnimationKeyFrame(frame, tex);
            }
        }
    }
}
