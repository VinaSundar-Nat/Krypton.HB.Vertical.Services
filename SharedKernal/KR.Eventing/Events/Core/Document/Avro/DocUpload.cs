// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.12.0+8c27801dc8d42ccc00997f25c0b8f45f8d4a233e
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Core.Document.Avro
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	/// <summary>
	/// Document upload success event.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("avrogen", "1.12.0+8c27801dc8d42ccc00997f25c0b8f45f8d4a233e")]
	public partial class DocUpload : global::Avro.Specific.ISpecificRecord
	{
		public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse(@"{""type"":""record"",""name"":""DocUpload"",""doc"":""Document upload success event."",""namespace"":""Core.Document.Avro"",""fields"":[{""name"":""doc_name"",""doc"":""Property describes the name of the document uploaded"",""type"":""string""},{""name"":""blob_path"",""doc"":""Property describes the uploaded path of the document (account : container)"",""type"":""string""},{""name"":""created_date"",""doc"":""Datetime when the document was uploaded"",""type"":""string""},{""name"":""meta_data"",""doc"":""metadata of the document uploaded"",""default"":null,""type"":{""type"":""record"",""name"":""MetaData"",""namespace"":""Core.Document.Avro"",""fields"":[{""name"":""version"",""doc"":""version of the uploaded document"",""type"":""string""},{""name"":""user"",""doc"":""user uploading the document"",""type"":""string""}]}}],""version"":""1""}");
		/// <summary>
		/// Property describes the name of the document uploaded
		/// </summary>
		private string _doc_name;
		/// <summary>
		/// Property describes the uploaded path of the document (account : container)
		/// </summary>
		private string _blob_path;
		/// <summary>
		/// Datetime when the document was uploaded
		/// </summary>
		private string _created_date;
		/// <summary>
		/// metadata of the document uploaded
		/// </summary>
		private Core.Document.Avro.MetaData _meta_data;
		public virtual global::Avro.Schema Schema
		{
			get
			{
				return DocUpload._SCHEMA;
			}
		}
		/// <summary>
		/// Property describes the name of the document uploaded
		/// </summary>
		public string doc_name
		{
			get
			{
				return this._doc_name;
			}
			set
			{
				this._doc_name = value;
			}
		}
		/// <summary>
		/// Property describes the uploaded path of the document (account : container)
		/// </summary>
		public string blob_path
		{
			get
			{
				return this._blob_path;
			}
			set
			{
				this._blob_path = value;
			}
		}
		/// <summary>
		/// Datetime when the document was uploaded
		/// </summary>
		public string created_date
		{
			get
			{
				return this._created_date;
			}
			set
			{
				this._created_date = value;
			}
		}
		/// <summary>
		/// metadata of the document uploaded
		/// </summary>
		public Core.Document.Avro.MetaData meta_data
		{
			get
			{
				return this._meta_data;
			}
			set
			{
				this._meta_data = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.doc_name;
			case 1: return this.blob_path;
			case 2: return this.created_date;
			case 3: return this.meta_data;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.doc_name = (System.String)fieldValue; break;
			case 1: this.blob_path = (System.String)fieldValue; break;
			case 2: this.created_date = (System.String)fieldValue; break;
			case 3: this.meta_data = (Core.Document.Avro.MetaData)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
