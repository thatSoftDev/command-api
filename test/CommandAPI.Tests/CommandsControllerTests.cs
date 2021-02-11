using System;
using System.Collections.Generic;
using Moq;
using AutoMapper;
using CommandAPI.Models;
using Xunit;
using CommandAPI.Controllers;
using CommandAPI.Data;
using CommandAPI.Profiles;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Dtos;

using Newtonsoft.Json.Serialization;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        Mock<ICommandAPIRepo> mockRepo;
        CommandsProfile realProfile;
        MapperConfiguration configuration;
        IMapper mapper;

        public CommandsControllerTests()
        {
            mockRepo = new Mock<ICommandAPIRepo>();
            realProfile = new CommandsProfile();
            configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);
        }

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            configuration = null;
            realProfile = null;
        }
        
        [Fact]
        public void GetAllCommands_ReturnsZeroResources_WhenDBIsEmpty()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetAllCommands()).Returns(GetCommands(0));

            var controller = new CommandsController(mockRepo.Object, mapper);

            //Act
            var result = controller.Get();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsOneItem_WhenDBHasOneResource() 
        {
            mockRepo.Setup(repo =>
                repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.Get();
            //Assert
            var okResult = result.Result as OkObjectResult;
            var commands = okResult.Value as List<CommandReadDto>;

            Assert.Single(commands);
        }

        [Fact]
        public void GetAllCommands_Returns200OK_WhenDBHasOneResources() 
        {
            mockRepo.Setup(repo =>
                repo.GetAllCommands()).Returns(GetCommands(1));
            
            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.Get();

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsCorrectType_WhenDBHasOneResource()
        {
            //Arrange
            mockRepo.Setup(repo =>
                repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act

            var result = controller.Get();
            //Assert
            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }

        private List<Command> GetCommands(int num)
        {
            var commands = new List<Command>();
            if (num > 0)
            {
                commands.Add(new Command
                {
                    Id = 0,
                    HowTo = "How to genrate a migration",
                    CommandLine = "dotnet ef migrations add <Name of Migration>",
                    Platform = ".Net Core EF"
                });
            }
            return commands;
        }
    }
}