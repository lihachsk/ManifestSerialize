﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.IO.Compression;

namespace ManifestSerialize
{
    [DataContract]
    internal class Parent
    {
        [DataMember]
        internal string UId;
        [DataMember]
        internal string Name;
    }
    [DataContract]
    internal class Shema
    {
        [DataMember]
        internal string UId;
        [DataMember]
        internal string Name;
        [DataMember]
        internal string ModifiedOnUtc;
        [DataMember]
        internal Parent Parent;
        [DataMember]
        internal string ManagerName;
        [DataMember]
        internal string Caption;
        public Shema()
        {
            Parent = new Parent();
        }
    }
    [DataContract]
    internal class Package
    {
        [DataMember]
        internal string Name;
        //[DataMember]
        //internal string UId;
        //[DataMember]
        //internal string PackageVersion;
        //[DataMember]
        //internal string ModifiedOnUtc;
        //[DataMember]
        //internal string Maintainer;
        //[DataMember]
        //internal List<string> DependsOn;
        //[DataMember]
        //internal List<Shema> Shemas;
        //public Package()
        //{
        //    Shemas = new List<Shema>();
        //    DependsOn = new List<string>();
        //}

    }
    [DataContract]
    internal class Packages_
    {
        [DataMember]
        internal List<Package> Packages;
        public Packages_()
        {
            Packages = new List<Package>();
        }
    }
    internal class Package_2
    {
        internal string name;
        internal int start;
        internal int end;
    }
    class Program
    {
        static string fileName = "Manifest.gz";
        static void Main(string[] args)
        {

            //Packages_ p = new Packages_();
            //p.Packages.Add(new Package()
            //{
            //    UId = "5be3998b-c5c3-42bb-a01c-2e4149059a97",
            //    PackageVersion = "7.7.0",
            //    Name = "Base",
            //    ModifiedOnUtc = "\\/Date(1430897816000)\\/",
            //    Maintainer = "Terrasoft",
            //    DependsOn = new List<string>() {
            //        "Base", "Main"
            //    },
            //    Shemas = new List<Shema>()
            //    {
            //        new Shema()
            //        {
            //            UId = "525c7ade-87a9-4869-b1cb-7c328e9e2338",
            //            Name = "AttributeValue",
            //            ModifiedOnUtc = "\\/Date(1365437323000)\\/",
            //            Parent = new Parent()
            //            {
            //                Name = "1bab9dcf-17d5-49f8-9536-8e0064f1dce0",
            //                UId = "BaseEntity"
            //            },
            //            ManagerName = "EntitySchemaManager",
            //            Caption = "Значение признака"
            //        }
            //    }
            //});
            //try
            //{
            //    using (Stream stream = File.Open("Test.gz", FileMode.Create))
            //    {
            //        using (GZipStream str = new GZipStream(stream, CompressionMode.Compress))
            //        {
            //            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Packages_));
            //            ser.WriteObject(str, p);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(string.Format("Ошибка при архивации/записи файла:{0}", ex.Message));
            //}
            //MemoryStream stream1 = new MemoryStream();
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Packages));

            //ser.WriteObject(stream1, p);

            //stream1.Position = 0;
            //StreamReader sr = new StreamReader(stream1);
            //Console.Write("JSON form of Person object: ");
            //Console.WriteLine(sr.ReadToEnd());

            //stream1.Position = 0;
            //Packages p2 = (Packages)ser.ReadObject(stream1);
            //foreach(var package in p2.Packages_)
            //{
            //    Console.Write("Deserialized back, got PackageName=");
            //    Console.Write(package.Name);
            //}
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {
                    using (GZipStream gz = new GZipStream(stream, CompressionMode.Decompress))
                    {

                        var sr = new StreamReader(gz);
                        int level = 0,
                            lineNumber = 0;
                        bool IsArray = false;
                        List<Package_2> listPack = new List<Package_2>();
                        Package_2 temp = null;
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine().Trim();
                            lineNumber++;
                            //Console.WriteLine(lineNumber.ToString());
                            //Console.WriteLine(line);
                            switch (line)
                            {
                                case "{":
                                    {
                                        level++;
                                        if (level == 2 && !IsArray)
                                        {
                                            if(temp != null)
                                            {
                                                listPack.Add(temp);
                                                temp = null;
                                            }
                                            temp = new Package_2();
                                            temp.start = lineNumber;
                                        }
                                    }
                                    break;
                                case "}":
                                case "},": 
                                    {
                                        if(level == 2 && temp != null && !IsArray)
                                        {
                                            temp.end = lineNumber;
                                            listPack.Add(temp);
                                            temp = null;
                                        }
                                        level--;
                                    }
                                    break;
                                case "[": if(level == 2)IsArray = true; break;
                                case "]":
                                case "],": if (level == 2) IsArray = false; break;
                                default:
                                    {
                                        string[] str = line.Split(':');
                                        if (str.Length >= 2)
                                        {
                                            switch(str[1].Trim())
                                            {
                                                case "[": if (level == 2) IsArray = true; break;
                                                case "]":
                                                case "],": if (level == 2) IsArray = false; break;
                                                case "{":level++;break;
                                                case "}":
                                                case "},": level--; break;
                                                default:
                                                    {
                                                        if (level == 2 && str[0].Trim() == "\"Name\"" && temp != null && !IsArray)
                                                        {
                                                            temp.name = str[1].Trim();
                                                        }
                                                    }
                                                    break;
                                            }
                                            
                                        }
                                    }
                                    break;
                            }
                        }
                        int i = 0;
                        foreach(var item in listPack)
                        {
                            Console.WriteLine("{0}:{1}({2};{3})", (++i).ToString(), item.name, item.start.ToString(), item.end.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Ошибка при разархивации/чтении файла:{0}", ex.Message));
            }
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {
                    using (GZipStream gz = new GZipStream(stream, CompressionMode.Decompress))
                    {

                        int i = 0;
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Packages_));
                        Packages_ p_ = (Packages_)ser.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(gz).ReadToEnd())));
                        foreach (var package in p_.Packages)
                        {
                            Console.WriteLine(string.Format("{0}:{1}", (++i).ToString(), package.Name));

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Ошибка при разархивации/чтении файла:{0}", ex.Message));
            }
            Console.ReadLine();
        }
    }
}
