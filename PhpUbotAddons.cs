using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UBotPlugin;

namespace PHPUbotAddons
{
    public class ExecuteFromFile : IUBotFunction
    {
        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        private string _returnValue = string.Empty;

        public ExecuteFromFile()
        {
            this._parameters.Add(new UBotParameterDefinition("PhpFile", UBotType.String));
            this._parameters.Add(new UBotParameterDefinition("PhpCompiler", UBotType.String));
            
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            try
            {
                string phpFile = Path.GetFullPath(parameters["PhpFile"]);
                string phpCompiler = Path.GetFullPath(parameters["PhpCompiler"]);
                
                //Check for php compiler
                if (File.Exists(phpCompiler))
                {
                    if (File.Exists(phpFile))
                    {
                        RunScript(phpCompiler, phpFile);
                    }
                    else
                    {
                        this._returnValue = "Php file specified doesn't exists";
                    }
                }
                else
                {
                    this._returnValue = "Php compiler specified doesn't exists";
                }
            }
            catch (Exception exception)
            {
                this._returnValue = exception.Message;
            }
        }
        
        public void RunScript(string phpCompiler, string phpFile)
        {
            try
            {
                if (phpFile.Length != 0)
                {
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = "\""+ phpCompiler + "\"";
                    start.Arguments = "\"" + phpFile + "\"";
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.RedirectStandardError = true;
                    start.CreateNoWindow = true;
                    
                    using (Process process = Process.Start(start))
                    {
                        using (StreamReader errorReader = process.StandardError) 
                        {
                            this._returnValue = errorReader.ReadToEnd();
                            if (this._returnValue != null && this._returnValue.Trim().Length > 0)
                                return;
                        }
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            this._returnValue = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._returnValue = ex.Message;
            }
        }

        
        public string Category
        {
            get
            {
                return "PHP Functions";
            }
        }

        public string FunctionName
        {
            get
            {
                return "Execute From File";
            }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get
            {
                return this._parameters;
            }
        }

        public object ReturnValue
        {
            get
            {
                return this._returnValue;
            }
        }

        public UBotType ReturnValueType
        {
            get
            {
                return UBotType.String;
            }
        }

        public UBotPlugin.UBotVersion UBotVersion
        {
            get
            {
                return UBotPlugin.UBotVersion.Standard;
            }
        }
    }

    public class ExecuteFromCode : IUBotFunction
    {
        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        private string _returnValue = string.Empty;

        public ExecuteFromCode()
        {
            this._parameters.Add(new UBotParameterDefinition("PhpCode", UBotType.String));
            this._parameters.Add(new UBotParameterDefinition("PhpCompiler", UBotType.String));
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            try
            {
                string phpCode = parameters["PhpCode"];
                string phpCompiler = Path.GetFullPath(parameters["PhpCompiler"]);

                //Check for php compiler
                if (File.Exists(phpCompiler))
                {
                    RunScript(phpCompiler, phpCode);
                }
                else
                {
                    this._returnValue = "Php compiler specified doesn't exists";
                }
            }
            catch (Exception ex)
            {
                this._returnValue = ex.Message;
            }
        }
        
        public void RunScript(string phpCompiler, string phpCode)
        {
            //Create a php file for the code
            string filePath = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid().ToString().Replace("-", "").Trim() + ".php";

            try
            {
                StreamWriter writer = File.CreateText(filePath);
                writer.WriteLine("<?php");
                writer.WriteLine(phpCode);
                writer.WriteLine("?>");
                writer.Flush();
                writer.Close();
                if (phpCode.Length != 0)
                {
                   
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = "\"" + phpCompiler + "\"";
                    start.Arguments = "\"" + filePath +"\"";
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.RedirectStandardError = true;
                    start.CreateNoWindow = true;
                    using (Process process = Process.Start(start))
                    {
                        using (StreamReader errorReader = process.StandardError)
                        {
                            this._returnValue = errorReader.ReadToEnd();
                            if (this._returnValue != null && this._returnValue.Trim().Length > 0)
                                return;
                        }

                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            this._returnValue = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._returnValue = ex.Message;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        public string Category
        {
            get
            {
                return "PHP Functions";
            }
        }

        public string FunctionName
        {
            get
            {
                return "Execute From Code";
            }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get
            {
                return this._parameters;
            }
        }

        public object ReturnValue
        {
            get
            {
                return this._returnValue;
            }
        }

        public UBotType ReturnValueType
        {
            get
            {
                return UBotType.String;
            }
        }

        public UBotPlugin.UBotVersion UBotVersion
        {
            get
            {
                return UBotPlugin.UBotVersion.Standard;
            }
        }
    }
}
