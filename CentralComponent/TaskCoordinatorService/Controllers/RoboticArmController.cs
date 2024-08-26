using Microsoft.AspNetCore.Mvc;

namespace TaskCoordinatorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoboticArmController : ControllerBase
    {
        private readonly IMessageBroker _taskMessageBroker;

        public RoboticArmController(IMessageBroker taskMessageBroker)
        {
            _taskMessageBroker = taskMessageBroker;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Robotic Arm API is running.");
        }

        /// <summary>
        /// Toggles the state of the robotic arm.
        /// </summary>
        /// <returns>A message indicating the new state of the robotic arm.</returns>
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleState(int armId)
        {
            // Logic to toggle the state of the robotic arm
            var task = new TaskModel() { RoboticArmId = armId, TaskType = 1 };
            await _taskMessageBroker.SendTaskAsync(task, CancellationToken.None, armId);

            return Ok("Robotic Arm state toggled.");
        }
    }
}
