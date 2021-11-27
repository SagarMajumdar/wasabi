using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using wasabi.Models;

namespace wasabi.Controllers
{

    [EnableCors("wasabi_allowed_origins")]
    [ApiController]
    public class WasabiController: ControllerBase 
    {
        private const string v = "/api/[controller]";
        private readonly AppDbContext _ctx;

        public WasabiController(AppDbContext ctx) {
            _ctx = ctx;
            _ctx.Database.EnsureCreated();
        }

        [HttpGet(v+"/chk")]
        public string Chk() {
            return "working ok";
        }

        [HttpGet(v+"/getusers")]
        // sync:

        // // public IEnumerable<User> getUsers() { return _ctx.Users.ToArray(); } 
        
        // public IActionResult GetAllUsers() {
        //     var users = _ctx.Users.ToList();
        //     return Ok(users);
        // }

        // async:
        public async  Task<IActionResult> GetAllUsers() {
            var users = await _ctx.Users.ToArrayAsync();
            return Ok(users);
        }

        [HttpGet(v+"/getuserwithid/{id}")]
        
        // if we send say 'xyz' instead of proper int Id then 400 -- bad request will be returned.
        public IActionResult GetUserWithId(int id) { 
            var user = _ctx.Users.Where( u =>  u.Id == id ).ToArray() ;
            
            if( user.Length == 0 ) { 
                return NotFound() ; // 404
            }
            return Ok(user); 
        }

    
        [HttpPost(v+"/upfl")]
        public async Task<IActionResult> Upfl(IFormCollection fl1, IFormFile fl )
        {
    
            Fl fle = new Fl();
            fle.FileName = fl.FileName;
            fle.FileType = fl.ContentType;

            MemoryStream ms = new MemoryStream();
            fl.CopyTo(ms);

            fle.Filedt = ms.ToArray();

            var files = await _ctx.Fls.ToArrayAsync();
            if(files.Length == 0) { 
                fle.FileId = 1;
            }
            else {
                fle.FileId = files.Length + 1; 
            }


            var user = await _ctx.Fls.AddAsync(fle);
            var result  = await _ctx.SaveChangesAsync();

            //var postedFile = Request.Form.Files;
            return NoContent();
        } 

        [HttpPost(v+"/saveUser")] 
        public async Task<IActionResult> CreateUser ([FromBody] User bodyv) {

            
            if( bodyv.Email == null ||  bodyv.Email.Trim() == "" &&  
                bodyv.Password == null  ||  bodyv.Password.Trim() == "" ) {
                    return BadRequest ("email or password cannot be blank");
                }
            // checking if the email is not already present
            User[] user_chk_email= await _ctx.Users.Where(u => u.Email == bodyv.Email ).ToArrayAsync();
            if( user_chk_email.Length  > 0)
            {
                 return BadRequest("email already regstered");    
            }

            // getting next user id 
            var users = await  _ctx.Users.ToArrayAsync();
            bodyv.JoinDate = DateTime.Now;
            bodyv.Id = users.Length + 1;


            // use bcrypt to hash pwd
            string pwd = BCrypt.Net.BCrypt.HashPassword(bodyv.Password ); 

            //bool verified = BCrypt.Net.BCrypt.Verify("<string plain pwd>", <hashed pwd>); 
        
            bodyv.Password = pwd;

            var user = await _ctx.Users.AddAsync(bodyv);
            await _ctx.SaveChangesAsync();


            // creating jwt token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key =Encoding.ASCII.GetBytes("wasabi_user_secret_123");
            SecurityTokenDescriptor tokenDescriptor  = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity( new Claim[] {
                    new Claim ( ClaimTypes.Email , bodyv.Email),
                    new Claim (ClaimTypes.Name, ( bodyv.FirstName + " " + bodyv.LastName)  )
                }),
                Expires= DateTime.UtcNow.AddMinutes(4),
                SigningCredentials = new SigningCredentials( 
                        new SymmetricSecurityKey(key) , SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token= tokenHandler.CreateToken(tokenDescriptor);
            string tkn  = tokenHandler.WriteToken(token);

         

            //

            return Ok(tkn);
        } 


        [HttpPost(v+"/updateuserabout")] 
        public async Task<IActionResult> UpdateUserAbout ([FromBody] User u ) 
        {

            // in body put dummy data for all fields that are not needed
            // {"id": 2, "about": "123456xyz", "email": "dummy_str", 
            // "firstname": "dummy_str", "lastname": "dummy_str", "password":"dummy_str"}
            
            int userid = u.Id;
            string abouttxt = u.About;

            var user = await  _ctx.Users.FindAsync(userid); // findasync(<primary identifier>)
            if( user != null ) {
                user.About = abouttxt;
                 _ctx.Users.Update(user);
                await _ctx.SaveChangesAsync();
            }
            
            return NoContent();
        }


        [HttpPost(v+"/userlogin")] 
        public async Task<IActionResult>  LoginUser( [FromBody] User u, [FromHeader] string authorization) 
        {
            Debug.WriteLine("🍈"+ u.Email + " " + u.Password);

            if ( authorization == null || authorization.Trim() == "" )
            {
                return BadRequest("unable to read authorization in header");
            }
            else{
                string auth_token = authorization.Split(" ")[1];

                if( u.Email != null &&  u.Email.Trim() != "" &&  
                    u.Password != null  && u.Password.Trim() != "") 
                {
                    User[] uarrs =  await _ctx.Users.Where(user=> user.Email ==  u.Email).ToArrayAsync();
                    if(uarrs.Length > 0) {
                        bool verified = BCrypt.Net.BCrypt.Verify(u.Password, uarrs[0].Password); 
                        if (verified) {
                            // validating jwt token
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var key = Encoding.ASCII.GetBytes("wasabi_user_secret_123");
                            try {
                                tokenHandler.ValidateToken(
                                    auth_token,
                                    new TokenValidationParameters
                                    {
                                        ValidateIssuerSigningKey = true,
                                        IssuerSigningKey = new SymmetricSecurityKey(key),
                                        ValidateIssuer = false,
                                        ValidateAudience = false,
                                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                                        ClockSkew = TimeSpan.Zero
                                    }, out SecurityToken validatedToken);

                                var jwtToken = (JwtSecurityToken)validatedToken;
                                var user_email = jwtToken.Claims.First(x => x.Type == "email").Value.ToString();
                                
                                return Ok(user_email);    
                            }
                            catch(Exception ex){
                                return BadRequest("the token could not be validated");
                            }
                            //
                        }
                        else {
                            return BadRequest("the entered username and/or password is not correct");
                        }
                    }
                    else {
                        return NotFound();    
                    }
                }
                else {
                    return BadRequest("email or password cannot be blank");

                }

            }

    
           // return NoContent();

        }
    }

}