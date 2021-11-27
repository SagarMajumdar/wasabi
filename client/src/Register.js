import React, { useState } from "react";
import wasabi from './assets/wasabi.png';
import axios from 'axios';


const Register=()=>{
    
    const [showuploadsection, setshowuploadsection] = useState(false);
    
    const [regtoggl, setRegtoggl] = useState(true);
    
    const [fname, setfname] = useState('');
    const [lname, setlname] = useState('');
    const [email, setemail] = useState('');
    const [about, setabout] = useState('');
    const [pwd, setpwd] = useState('');
   
    

    const [loginemail, setloginemail] = useState('');
    const [loginpass, setloginpass] = useState('');
    const [loginshowpwd, setloginshowpwd] = useState(false);
    

    const [demofl,setdemofl]=useState();


    const submitLogin =async (e)=> {
        e.preventDefault();
        
        if(loginemail != "" && loginpass != "") {
            const result =  await axios.post ('https://localhost:7220/api/wasabi/userlogin' , 
                        { email:loginemail,  password:loginpass}, 
                        {headers: { authorization: 'Bearer '+ localStorage.getItem('wasabi_access_token') } } )
           if(result.status == 200) {
               console.log('🌵 user ok');
               setshowuploadsection(true);
           }
        }
    }

    const submitReg  = async (e)=>{
        e.preventDefault();
        
        if(fname != '' && lname != '' && email != '' && about != '' && pwd != '') {
            const result =  await axios.post ('https://localhost:7220/api/wasabi/saveuser' , {firstname:fname, lastname:lname, email:email, about:about, password:pwd})
            if(result.status == 200) 
            {   
                console.log('🍣 user registered completed');
                localStorage.setItem('wasabi_access_token', result.data);
            }
        }
    }

    return (
        <>      
                <div className="columns is-vcentered">
                    <div className="column is-1"></div>
                    <div className="column is-1"><img src={wasabi} alt="wasabi"></img></div>
                    <div className="column is-2"> <h4 className="title is-4">wasabi</h4> </div>
                    <div className="column"></div>
                </div>
                    
                { regtoggl ?
                    <>
                         
                        <div className="columns">
                            <div className="column is-1"></div>
                            <div className="column is-1"><h5 className="title is-5">register</h5> </div>
                            <div className="column"></div>
                        </div>
                    
                        <form onSubmit={submitReg} >  {/* register */}

                            <div className="columns">
                                <div className="column is-1"></div>
                                <div className="column is-1">firstname</div>
                                <div className="column is-2"><input onChange={(e)=>{ setfname(e.target.value) }} value={fname} className="input is-small"  type="text" ></input></div>
                                <div className="column is-1">lastname</div>
                                <div className="column is-2"><input onChange={(e)=>{ setlname(e.target.value) }} value={lname} className="input is-small" type="text"></input></div>
                                <div className="column"></div>
                            </div>
                            <div className="columns">
                                <div className="column is-1"></div>
                                <div className="column is-1">email (username)</div>
                                <div className="column is-2"><input onChange={(e)=>{ setemail(e.target.value) }} value={email}  className="input is-small"  type="text"></input></div>
                                <div className="column"></div>
                            </div>   
                              
                            <div className="columns">
                                <div className="column is-1"></div>
                                <div className="column is-1">about</div>
                                <div className="column is-5"><textarea onChange={(e)=>{ setabout(e.target.value) }} value={about} rows="1" className="textarea" ></textarea></div>
                                <div className="column"></div>
                            </div>   
                            <div className="columns">
                                <div className="column is-1"></div>
                                <div className="column is-1">password</div>
                                <div className="column is-2"><input onChange={(e)=>{ setpwd(e.target.value) }} value={pwd}  className="input is-small"  type="text"></input></div>
                                <div className="column"></div>
                            </div>
                            <div className="columns">
                                <div className="column is-1"></div>
                                <div className="column is-1"></div>
                                <div className="column is-2"><button  className="button is-small is-success is-outlined"  type="submit">register</button></div>
                                <div className="column"></div>
                            </div>
                            
                        </form>
                    </>
                    :
                    <>
                        <div className="columns">
                            <div className="column is-1"></div>
                            <div className="column is-1"><h5 className="title is-5">login</h5> </div>
                            <div className="column"></div>
                        </div>
                        <form onSubmit={submitLogin}>{/* login */}
                            <div className="columns">
                                <div className="column is-1"></div>
                                <div className="column is-1">email</div>
                                <div className="column is-2"><input onChange={(e)=>{setloginemail(e.target.value) }} value={loginemail} className="input is-small"  type="text" ></input></div>
                                <div className="column is-1">password</div>
                                <div className="column is-2">
                                    <input onChange={(e)=>{ setloginpass(e.target.value) }} value={loginpass} className="input is-small" type={ loginshowpwd ? "text" : "password"}></input>
                                    
                                </div>
                                <div className="column is-1">
                                     <button type="button" className="button is-outlined is-rounded is-dark is-small" onClick={()=>{ setloginshowpwd(p=>!p)}} >show/hide password</button>
                                </div>
                                <div className="column"></div>
                            </div>
                            <div className="columns">
                                <div className="column is-1"></div>
                                <div className="column is-1"></div>
                                <div className="column is-2"><button  className="button is-small is-success is-outlined"  type="submit">login</button></div>
                                <div className="column"></div>
                            </div>
                        </form>
                    </>
                }

                
                <div className="columns">
                    <div className="column is-2"></div>
                    <div className="column is-2"> <button className="button is-small is-warning" type="button" onClick={ ()=> { setRegtoggl( p => !p)} } > toggle login/register</button></div>
                    <div className="column"></div>
                </div>
                <div className="columns">
                    <div className="column is-2"></div>
                    <div className="column is-2"> 
                        <div className="content is-small">
                            <span className="is-small tag is-info" style={{marginRight:'5px'}}>i</span> 
                            <p>demo for simple jwt auth + file upload using asp.net core web api. </p>
                            <p>Register and login for seeing upload 
                            form.</p>
                            <p>after successfull registration a message is logged in console. Same for successfull login.</p> 
                            <div>Icons made by <a href="https://www.flaticon.com/authors/icongeek26" title="Icongeek26">Icongeek26</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
                        </div>
                    </div>
                    <div className="column"></div>
                </div>
               
                {
                    showuploadsection
                    &&
                    <div  className="columns">
                    <div className="column is-2"></div>
                        <div className="column is-2"> 

                            <form onSubmit= { 
                                async (e) => {
                                    e.preventDefault();

                                    const formdt= new FormData();
                                            formdt.append("fl", demofl[0]);
                                            formdt.append("test", "seding other data wth file text value");
                                    
                                            const result =  await axios.post ('https://localhost:7220/api/wasabi/upfl' ,
                                                formdt , {headers:{'Content-Type':'multipart/form-data'}}) ;
                                                    console.log('🍚');
                                                    console.log(result);

                                        }
                                    }>
                                    <input type="file" className="input file is-small" name="infile" onChange={(e)=>{  setdemofl(e.target.files) }}></input> 
                                    <button type="submit" className ="button is-small is-warning ">submit</button>
                            </form>

                        </div>
                        <div className="column"></div>
                    </div>
                }
               


        </>
    )
}

export default Register;