using System;
using Xunit;
using TeamUp;
using Models;

namespace TeamUp.Test
{
    public class VerificationCodeTests
    {
        [Fact]
        public void GenerateEmailVerificationCode_ValidCodeGenerated()
        {
            // Arrange
            var verificationCode = VerificationCode.CreateEmailVerificationCode();

            // Act
            var generatedCode = verificationCode.Code;

            // Assert
            Assert.NotNull(generatedCode);
            Assert.Equal(6, generatedCode.Length); // Assuming generated code is always 6 digits
        }

        [Fact]
        public void GeneratePasswordRestCode_ValidCodeGenerated()
        {
            // Arrange
            var verificationCode = VerificationCode.CreatePasswordResetCode();
            

            // Act
            var generatedCode = verificationCode.Code;

            // Assert
            Assert.NotNull(generatedCode);
            Assert.Equal(6, generatedCode.Length); // Assuming generated code is always 6 digits
        }

        [Fact]
        public void IsEmailVerificationCode_EmailVerificationType_True()
        {
            // Arrange
            var verificationCode = VerificationCode.CreateEmailVerificationCode();

            // Act
            var isEmailVerificationCode = verificationCode.IsEmailVerificationCode();

            // Assert
            Assert.True(isEmailVerificationCode);
        }

        [Fact]
        public void IsPasswordRestCode_PasswordRestType_True()
        {
            // Arrange
            var verificationCode = VerificationCode.CreatePasswordResetCode();

            // Act
            var isPasswordRestCode = verificationCode.IsPasswordRestCode();

            // Assert
            Assert.True(isPasswordRestCode);
        }

        [Fact]
        public void IsValid_ValidCodeAndNotExpired_True()
        {
            // Arrange
            var verificationCode = VerificationCode.Create("123456",10);

            // Act
            var isValid = verificationCode.IsValid("123456");

System.Console.WriteLine("wow");
            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_ExpiredCode_False()
        {
            // Arrange
            var verificationCode = VerificationCode.Create("123456",-1);

            // Act
            var isValid = verificationCode.IsValid("123456");

            // Assert
            Assert.False(isValid);

            System.Console.WriteLine("\n\n Success");
        }
    }
}
