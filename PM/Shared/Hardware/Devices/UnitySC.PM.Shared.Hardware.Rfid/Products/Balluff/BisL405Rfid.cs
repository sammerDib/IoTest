using System;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Rfids;
using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Rfid
{
    public class BisL405Rfid : RfidBase
    {
        private readonly BisL405RfidController _controller;
        private readonly BisL405RfidConfig _config;
        private readonly ILogger _logger;

        public BisL405Rfid(IGlobalStatusServer globalStatusServer, ILogger logger, RfidConfig config, RfidController controller) :
            base(globalStatusServer, logger, config, controller)
        {
            _logger = logger;
            if (config is BisL405RfidConfig rfidConfig)
            {
                _config = rfidConfig;
            }
            else
            {
                throw new InvalidOperationException($"Invalid configuration type: {config.GetType()}");
            }

            if (controller is BisL405RfidController rfidController)
            {
                _controller = rfidController;
            }
            else
            {
                throw new InvalidOperationException($"Invalid controller type: {controller.GetType()}");
            }

        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        private Length GetChuckSizeByTag(string detectedTag)
        {
            if (!detectedTag.IsNullOrEmpty())
            {

                return _config.RfidTags.Where(tag => tag.Message.Contains(detectedTag))
                                            .Select(tag => tag.Size)
                                            .FirstOrDefault();
            }
            else
            {
                return 0.Millimeters();
            }
        }

        public override RfidTag GetTag()
        {
            _controller.ReadTag();

            string detectedTag = _controller.GetTag();

            if (!string.IsNullOrWhiteSpace(detectedTag) && !TagIsInConfiguration(detectedTag))
            {
                string message = $"Unknown Tag. The tag {detectedTag} is not in authorised list of tags ('<RfidTags>').";
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(new Message(MessageLevel.Error, message)));

                _logger.Error(message);
                detectedTag = null;

            }
            else if (string.IsNullOrWhiteSpace(detectedTag))
            {
                string message = "The RFID tag could not be read";
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(new Message(MessageLevel.Error, message)));

                _logger.Error(message);
                detectedTag = null;
            }

            var tag = new RfidTag
            {
                Message = detectedTag,
                Size = GetChuckSizeByTag(detectedTag)
            };

            return tag;
        }

        private bool TagIsInConfiguration(string tag)
        {
            return _config.RfidTags.Exists(tagConf => tagConf.Message.Contains(tag));
        }
    }
}
