﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
namespace FileManager
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class FileSystemLibraryEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new FileSystemLibraryEntities object using the connection string found in the 'FileSystemLibraryEntities' section of the application configuration file.
        /// </summary>
        public FileSystemLibraryEntities() : base("name=FileSystemLibraryEntities", "FileSystemLibraryEntities")
        {
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new FileSystemLibraryEntities object.
        /// </summary>
        public FileSystemLibraryEntities(string connectionString) : base(connectionString, "FileSystemLibraryEntities")
        {
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new FileSystemLibraryEntities object.
        /// </summary>
        public FileSystemLibraryEntities(EntityConnection connection) : base(connection, "FileSystemLibraryEntities")
        {
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Directory> Directories
        {
            get
            {
                if ((_Directories == null))
                {
                    _Directories = base.CreateObjectSet<Directory>("Directories");
                }
                return _Directories;
            }
        }
        private ObjectSet<Directory> _Directories;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<File> Files
        {
            get
            {
                if ((_Files == null))
                {
                    _Files = base.CreateObjectSet<File>("Files");
                }
                return _Files;
            }
        }
        private ObjectSet<File> _Files;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Directories EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToDirectories(Directory directory)
        {
            base.AddObject("Directories", directory);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Files EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToFiles(File file)
        {
            base.AddObject("Files", file);
        }

        #endregion

        #region Function Imports
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public int ResetData()
        {
            return base.ExecuteFunction("ResetData");
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="FileSystemLibraryModel", Name="Directory")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Directory : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Directory object.
        /// </summary>
        /// <param name="directoryID">Initial value of the DirectoryID property.</param>
        /// <param name="directoryName">Initial value of the DirectoryName property.</param>
        /// <param name="ignore">Initial value of the Ignore property.</param>
        public static Directory CreateDirectory(global::System.Int64 directoryID, global::System.String directoryName, global::System.Boolean ignore)
        {
            Directory directory = new Directory();
            directory.DirectoryID = directoryID;
            directory.DirectoryName = directoryName;
            directory.Ignore = ignore;
            return directory;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 DirectoryID
        {
            get
            {
                return _DirectoryID;
            }
            set
            {
                if (_DirectoryID != value)
                {
                    OnDirectoryIDChanging(value);
                    ReportPropertyChanging("DirectoryID");
                    _DirectoryID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("DirectoryID");
                    OnDirectoryIDChanged();
                }
            }
        }
        private global::System.Int64 _DirectoryID;
        partial void OnDirectoryIDChanging(global::System.Int64 value);
        partial void OnDirectoryIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String DirectoryName
        {
            get
            {
                return _DirectoryName;
            }
            set
            {
                OnDirectoryNameChanging(value);
                ReportPropertyChanging("DirectoryName");
                _DirectoryName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("DirectoryName");
                OnDirectoryNameChanged();
            }
        }
        private global::System.String _DirectoryName;
        partial void OnDirectoryNameChanging(global::System.String value);
        partial void OnDirectoryNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> ParentID
        {
            get
            {
                return _ParentID;
            }
            set
            {
                OnParentIDChanging(value);
                ReportPropertyChanging("ParentID");
                _ParentID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ParentID");
                OnParentIDChanged();
            }
        }
        private Nullable<global::System.Int64> _ParentID;
        partial void OnParentIDChanging(Nullable<global::System.Int64> value);
        partial void OnParentIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Boolean Ignore
        {
            get
            {
                return _Ignore;
            }
            set
            {
                OnIgnoreChanging(value);
                ReportPropertyChanging("Ignore");
                _Ignore = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Ignore");
                OnIgnoreChanged();
            }
        }
        private global::System.Boolean _Ignore;
        partial void OnIgnoreChanging(global::System.Boolean value);
        partial void OnIgnoreChanged();

        #endregion

    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="FileSystemLibraryModel", Name="File")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class File : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new File object.
        /// </summary>
        /// <param name="fileID">Initial value of the FileID property.</param>
        /// <param name="fileName">Initial value of the FileName property.</param>
        /// <param name="directoryID">Initial value of the DirectoryID property.</param>
        /// <param name="ignore">Initial value of the Ignore property.</param>
        public static File CreateFile(global::System.Int64 fileID, global::System.String fileName, global::System.Int64 directoryID, global::System.Boolean ignore)
        {
            File file = new File();
            file.FileID = fileID;
            file.FileName = fileName;
            file.DirectoryID = directoryID;
            file.Ignore = ignore;
            return file;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 FileID
        {
            get
            {
                return _FileID;
            }
            set
            {
                if (_FileID != value)
                {
                    OnFileIDChanging(value);
                    ReportPropertyChanging("FileID");
                    _FileID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("FileID");
                    OnFileIDChanged();
                }
            }
        }
        private global::System.Int64 _FileID;
        partial void OnFileIDChanging(global::System.Int64 value);
        partial void OnFileIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                OnFileNameChanging(value);
                ReportPropertyChanging("FileName");
                _FileName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("FileName");
                OnFileNameChanged();
            }
        }
        private global::System.String _FileName;
        partial void OnFileNameChanging(global::System.String value);
        partial void OnFileNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 DirectoryID
        {
            get
            {
                return _DirectoryID;
            }
            set
            {
                OnDirectoryIDChanging(value);
                ReportPropertyChanging("DirectoryID");
                _DirectoryID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("DirectoryID");
                OnDirectoryIDChanged();
            }
        }
        private global::System.Int64 _DirectoryID;
        partial void OnDirectoryIDChanging(global::System.Int64 value);
        partial void OnDirectoryIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Checksum
        {
            get
            {
                return _Checksum;
            }
            set
            {
                OnChecksumChanging(value);
                ReportPropertyChanging("Checksum");
                _Checksum = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Checksum");
                OnChecksumChanged();
            }
        }
        private global::System.String _Checksum;
        partial void OnChecksumChanging(global::System.String value);
        partial void OnChecksumChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> CreationTime
        {
            get
            {
                return _CreationTime;
            }
            set
            {
                OnCreationTimeChanging(value);
                ReportPropertyChanging("CreationTime");
                _CreationTime = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreationTime");
                OnCreationTimeChanged();
            }
        }
        private Nullable<global::System.DateTime> _CreationTime;
        partial void OnCreationTimeChanging(Nullable<global::System.DateTime> value);
        partial void OnCreationTimeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> CreationTimeUTC
        {
            get
            {
                return _CreationTimeUTC;
            }
            set
            {
                OnCreationTimeUTCChanging(value);
                ReportPropertyChanging("CreationTimeUTC");
                _CreationTimeUTC = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreationTimeUTC");
                OnCreationTimeUTCChanged();
            }
        }
        private Nullable<global::System.DateTime> _CreationTimeUTC;
        partial void OnCreationTimeUTCChanging(Nullable<global::System.DateTime> value);
        partial void OnCreationTimeUTCChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> Length
        {
            get
            {
                return _Length;
            }
            set
            {
                OnLengthChanging(value);
                ReportPropertyChanging("Length");
                _Length = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Length");
                OnLengthChanged();
            }
        }
        private Nullable<global::System.Int64> _Length;
        partial void OnLengthChanging(Nullable<global::System.Int64> value);
        partial void OnLengthChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                OnErrorMessageChanging(value);
                ReportPropertyChanging("ErrorMessage");
                _ErrorMessage = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ErrorMessage");
                OnErrorMessageChanged();
            }
        }
        private global::System.String _ErrorMessage;
        partial void OnErrorMessageChanging(global::System.String value);
        partial void OnErrorMessageChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Boolean Ignore
        {
            get
            {
                return _Ignore;
            }
            set
            {
                OnIgnoreChanging(value);
                ReportPropertyChanging("Ignore");
                _Ignore = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Ignore");
                OnIgnoreChanged();
            }
        }
        private global::System.Boolean _Ignore;
        partial void OnIgnoreChanging(global::System.Boolean value);
        partial void OnIgnoreChanged();

        #endregion

    
    }

    #endregion

    
}
