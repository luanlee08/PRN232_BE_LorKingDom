using BLL.DTOs;
using BLL.DTOs.Templates;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/templates")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ATemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public ATemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        /// <summary>
        /// Get all templates with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TemplateResponse>>>> GetTemplates([FromQuery] TemplateQuery query)
        {
            var result = await _templateService.GetAsync(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get template by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TemplateResponse>>> GetTemplateById(short id)
        {
            var result = await _templateService.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get template by code
        /// </summary>
        [HttpGet("by-code/{templateCode}")]
        public async Task<ActionResult<ApiResponse<TemplateResponse>>> GetTemplateByCode(string templateCode)
        {
            var result = await _templateService.GetByCodeAsync(templateCode);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get all active templates
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<TemplateResponse>>>> GetActiveTemplates()
        {
            var result = await _templateService.GetActiveAsync();
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Create a new template
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<short>>> CreateTemplate([FromBody] CreateTemplateRequest request)
        {
            var result = await _templateService.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Update template
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTemplate(short id, [FromBody] UpdateTemplateRequest request)
        {
            var result = await _templateService.UpdateAsync(id, request);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Toggle template status (activate/deactivate)
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleStatus(short id)
        {
            var result = await _templateService.ToggleStatusAsync(id);
            return StatusCode(result.Status, result);
        }
    }
}
