using EasyCqrs.Mediator;
using EasyCqrs.Mvc;
using Xunit;

namespace EasyCqrs.Tests;
public class SensitiveAttributeTests
{
    [Fact]
    public void SensitiveStringAttributeShouldBeMasked()
    {
        //arrange
        var sensitiveTest = new SensitiveClassTest
        {
            Email = "test@gmail.com",
            Password = "testPassword"
        };

        //act
        sensitiveTest.MaskSensitiveStrings();   

        //assert
        Assert.Equal("*", sensitiveTest.Password);
    }

    private class SensitiveClassTest
    {
        public string? Email { get; set; }
        [Sensitive] public string? Password { get; set; }
    }
}
