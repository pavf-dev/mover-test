using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using MoverCandidateTest.Application.WatchHands;

namespace MoverCandidateTest.Controllers.WatchHands
{
    [ApiController]
    [Route("[controller]")]
    public class CalculateLeastAngleController : ControllerBase
    {
        private readonly WatchHandsAngleDifferenceCalculator _watchHandsAngleDifferenceCalculator;

        public CalculateLeastAngleController(WatchHandsAngleDifferenceCalculator watchHandsAngleDifferenceCalculator)
        {
            _watchHandsAngleDifferenceCalculator = watchHandsAngleDifferenceCalculator;
        }

        [HttpGet]
        public string Get([FromQuery] CalculateLeastAngleRequestModel requestModel)
        {
            return _watchHandsAngleDifferenceCalculator.GetLeastAnglesDifference(TimeOnly.FromDateTime(requestModel.DateTime)).ToString(CultureInfo.CurrentCulture);
        }
    }
}
