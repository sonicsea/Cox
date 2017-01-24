using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cox.ViewModels;
using Cox.Models;
using System.Security.Cryptography;
using System.Text;

namespace Cox.Helpers
{
    public static class Security
    {
        public static int Authenticate(UserAuthViewModel userAuth)
        {
            try
            {
                if (!EmailAlreadyExists(userAuth.Email)) return -1;

                var context = new CoxEntities();

                string password = GetHash(userAuth.Email);
                byte[] salt = GetSalt(userAuth.Email);


                byte[] enteredPassword = Encoding.ASCII.GetBytes(userAuth.Password);
                byte[] enteredHash = GenerateSaltedHash(enteredPassword, salt);

                if (password == Encoding.ASCII.GetString(enteredHash))
                {
                    UpdateLastLoginTime(userAuth.Email);
                    return GetUserID(userAuth.Email);
                }

                else
                    return -1;
            }
            catch(Exception ex)
            {
                throw new Exception("Error from Authenticate ", ex.InnerException);
            }
                
        }

        private static void UpdateLastLoginTime(string email)
        {
            try
            {
                var context = new CoxEntities();

                var user = context.Users.Where(u => u.Email == email);

                if (user != null)
                {
                    user.First().LastLogin = DateTime.Now;
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error from UpdateLastLoginTime ", ex.InnerException);
            }
        }

        public static int GetUserID(string email)
        {
            try
            {
                var context = new CoxEntities();

                var user = from u in context.Users
                           where u.Email == email
                           select u.ID;

                return user.First();
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetUserID ", ex.InnerException);
            }
        }
        public static int Register(Cox.ViewModels.RegisterViewModel newUser)
        {
            try
            {
                var context = new CoxEntities();

                User user = new User();

                user.Email = newUser.Email;
                user.Salt = CreateSalt(100);
                user.Password = Encoding.ASCII.GetString(GenerateSaltedHash(Encoding.ASCII.GetBytes(newUser.Password), Encoding.ASCII.GetBytes(user.Salt)));
                user.FirstName = newUser.FirstName;
                user.LastName = newUser.LastName;
                user.SupervisorEmail = newUser.SupervisorEmail;
                user.CreatedOn = DateTime.Now;

                context.Users.Add(user);



                context.SaveChanges();

                UpdateLastLoginTime(newUser.Email);

                return GetUserID(newUser.Email);
            }
            catch(Exception ex)
            {
                throw new Exception("Error from Register: " + ex.Message, ex.InnerException);
            }

        }

        public static void CreateNewResetPWSession(int userID, string token)
        {
            try
            {

                int resetPeriod = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PW_Reset_Time"]);
                using (var context = new CoxEntities())
                {
                    passwordreset pr = new passwordreset();
                    pr.User_ID = userID;
                    pr.Token = token;
                    pr.CreateTime = DateTime.Now;
                    pr.ExpireTime = DateTime.Now.AddMinutes(resetPeriod);
                    pr.IsExpired = 0;

                    context.passwordresets.Add(pr);

                    context.SaveChanges();
                }


            }
            catch (Exception ex)
            {
                throw new Exception("Error ResetPassword: " + ex.Message, ex.InnerException);
            }
        }

        public static void ResetPassword(int userID, string token, string password)
        {
            try
            {
                

                using (var context = new CoxEntities())
                {
                    User user = context.Users.Where(u => u.ID == userID).FirstOrDefault();
                    user.Password = Encoding.ASCII.GetString(GenerateSaltedHash(Encoding.ASCII.GetBytes(password), Encoding.ASCII.GetBytes(user.Salt)));
                    user.UpdatedOn = DateTime.Now;

                    passwordreset pr = context.passwordresets.Where(p => p.User_ID == userID && p.Token == token).FirstOrDefault();

                    pr.IsExpired = 1;

                    context.SaveChanges();
                }

                
            }
            catch (Exception ex)
            {
                throw new Exception("Error ResetPassword: " + ex.Message, ex.InnerException);
            }
        }

        public static bool IsResetSessionExpired(int userID, string token)
        {
            try
            {
                bool isExpired = true;

                using (var context = new CoxEntities())
                {
                    passwordreset pr = context.passwordresets.Where(p => p.User_ID == userID && p.Token == token).FirstOrDefault();

                    if (pr.IsExpired == 1) return true;

                    if (DateTime.Now >= pr.CreateTime && DateTime.Now <= pr.ExpireTime)
                    {
                        isExpired = false;
                    }
                    else
                    {

                        pr.IsExpired = 1;

                        context.SaveChanges();
                    }
                }

                return isExpired;
            }
            catch(Exception ex)
            {
                throw new Exception("Error IsResetSessionExpired: " + ex.Message, ex.InnerException);
            }
        }

        public static User GetUser(int userID)
        {
            try
            {
                var context = new CoxEntities();

                var user = from u in context.Users
                           where u.ID == userID
                           select u;

                return user.First();
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetUser ", ex.InnerException);
            }
        }

        public static bool EmailAlreadyExists(string email)
        {
            try
            {
                var context = new CoxEntities();

                var user = from u in context.Users
                           where u.Email == email
                           select u;

                if (user.ToList().Count > 0) return true;

                return false;
            }
            catch(Exception ex)
            {
                throw new Exception("Error from EmailAlreadyExists ", ex.InnerException);
            }
        }

        private static byte[] GetSalt(string email)
        {
            try
            {
                var context = new CoxEntities();

                var salt = from u in context.Users
                           where u.Email == email
                           select u.Salt;

                return Encoding.ASCII.GetBytes(salt.First());
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetSalt ", ex.InnerException);
            }
        }

        private static string GetHash(string email)
        {
            try
            {
                var context = new CoxEntities();

                var hash = from u in context.Users
                           where u.Email == email
                           select u.Password;

                return hash.First();
            }
            catch (Exception ex)
            {
                throw new Exception("Error from GetHash ", ex.InnerException);
            }

        }

        public static string CreateSalt(int size)
        {
            try
            {
                //Generate a cryptographic random number.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buff = new byte[size];
                rng.GetBytes(buff);

                // Return a Base64 string representation of the random number.
                return Convert.ToBase64String(buff);
            }
            catch (Exception ex)
            {
                throw new Exception("Error from CreateSalt: " + ex.Message, ex.InnerException);
            }
        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            try
            {
                HashAlgorithm algorithm = new SHA256Managed();

                byte[] plainTextWithSaltBytes =
                  new byte[plainText.Length + salt.Length];

                for (int i = 0; i < plainText.Length; i++)
                {
                    plainTextWithSaltBytes[i] = plainText[i];
                }
                for (int i = 0; i < salt.Length; i++)
                {
                    plainTextWithSaltBytes[plainText.Length + i] = salt[i];
                }

                return algorithm.ComputeHash(plainTextWithSaltBytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Error from GetHash ", ex.InnerException);
            }
        }
        
    }
}