using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : Controller
{
    private readonly ProjectService _projectService;

    List<string> _errorMessages = new List<string>();

    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
    // {
    //     IEnumerable<ProjectDTO> projectsDTO = await _projectService.GetAll();
    //     return Ok(projectsDTO);
    // }

    // [HttpGet("{id}")]
    // private async Task<ActionResult<ProjectDTO>> GetProjectById(long id)
    // {
    //     var projectDTO = await _projectService.GetById(id);
    //     if (projectDTO is not null)
    //     {
    //         return Ok(projectDTO);
    //     }
    //
    //     return NotFound();
    // }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProject(long id, ProjectDTO projectDTO)
    {
        if (id != projectDTO.Id)
        {
            return BadRequest();
        }

        bool wasUpdated = await _projectService.Update(id, projectDTO, _errorMessages, true);
        if (!wasUpdated)
        {
            return BadRequest(_errorMessages);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<ProjectDTO>>> PostProject(ProjectDTO projectDTO)
    {
        ProjectDTO projectResultDTO = await _projectService.AddFromRest(projectDTO, _errorMessages);
        if (projectResultDTO is not null)
        {
            return Created("", projectResultDTO);
        }
        else
        {
            return BadRequest(_errorMessages);
        }
    }
}
