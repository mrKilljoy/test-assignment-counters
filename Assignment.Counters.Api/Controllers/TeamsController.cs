using System.ComponentModel.DataAnnotations;
using Assignment.Counters.Application.Interfaces;
using Assignment.Counters.Application.Models.Requests;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Counters.Api.Controllers;

/// <summary>
/// The API used to work with teams.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TeamsController : BaseController
{
    private readonly ITeamManager _teamManager;
    private readonly ICounterManager _counterManager;

    public TeamsController(ITeamManager teamManager, ICounterManager counterManager)
    {
        _teamManager = teamManager;
        _counterManager = counterManager;
    }
    
    /// <summary>
    /// Provides the total number of steps from all counters for a team.
    /// </summary>
    /// <param name="id">The team ID.</param>
    /// <returns>The response with the team name and total number of steps.</returns>
    [Route("{id}/steps")]
    [HttpGet]
    public async Task<IActionResult> GetSteps([FromRoute]Guid id)
    {
        try
        {
            var result = await _teamManager.Get(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }
    
    /// <summary>
    /// Provides the full list of teams and their total steps.
    /// </summary>
    /// <returns>The list of all teams and their total steps in descending order.</returns>
    [Route("leaderboard")]
    [HttpGet]
    public async Task<IActionResult> GetLeaderboard()
    {
        try
        {
            var result = await _teamManager.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }
    
    /// <summary>
    /// Provides the list of all counters for a specific team.
    /// </summary>
    /// <param name="id">The team ID.</param>
    /// <returns>The list of all counters in the team.</returns>
    [Route("{id}/counters")]
    [HttpGet]
    public async Task<IActionResult> GetCounters([FromRoute]Guid id)
    {
        try
        {
            var result = await _counterManager.GetCounters(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }

    /// <summary>
    /// Creates a team.
    /// </summary>
    /// <param name="request">The request model with information for the new team.</param>
    /// <param name="validator">The request model validator.</param>
    /// <returns>Operation result.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTeam(
        [FromBody][Required]NewTeamRequest request,
        [FromServices]IValidator<NewTeamRequest> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }
            
            var result = await _teamManager.Create(request.TeamName);
            return Created(new Uri(Request.GetEncodedUrl()+ "/" + result.Id), result.Id);
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }
    
    /// <summary>
    /// Deletes a team.
    /// </summary>
    /// <param name="id">The team ID.</param>
    /// <returns>Operation result.</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteTeam([FromRoute]Guid id)
    {
        try
        {
            await _teamManager.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }
}