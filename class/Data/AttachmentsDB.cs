//
// Active Forums - http://www.dnnsoftware.com
// Copyright (c) 2013
// by DNN Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class AttachController : Connection
	{
        public int Save(int contentId, int userId, string fileName, string contentType, long fileSize, int? fileId)
        {
             return Convert.ToInt32(SqlHelper.ExecuteScalar(connectionString, dbPrefix + "Attachments_Save", contentId, userId, fileName, contentType, fileSize, fileId));
        }

        public void Delete(int attachId)
        {
            SqlHelper.ExecuteNonQuery(connectionString, dbPrefix + "Attachments_Delete", attachId);
        }

        // FileId is used for legacy attachments
        public PermissionAttachment Get(int attachmentId, int fileId, bool withSecurity)
        {
            PermissionAttachment result = null;
            using (IDataReader dr = SqlHelper.ExecuteReader(connectionString, dbPrefix + "Attachments_Get", attachmentId, fileId, withSecurity))
            {
                if (dr.Read())
                {
                    result = FillAttachment(dr);
                }
                dr.Close();
            }

            return result;
        }

        public List<Attachment> ListForContent(int contentId)
        {
            var al = new List<Attachment>();
            using (IDataReader dr = SqlHelper.ExecuteReader(connectionString, dbPrefix + "Attachments_ListForContent", contentId))
            {
                while (dr.Read())
                {
                    al.Add(FillAttachment(dr));
                }
                dr.Close();
            }
            return al;
        }

        public List<Attachment> ListForPost(int topicId, int? replyId)
        {
            var al = new List<Attachment>();
            using (IDataReader dr = SqlHelper.ExecuteReader(connectionString, dbPrefix + "Attachments_ListForPost", topicId, replyId))
            {
                while (dr.Read())
                {
                    al.Add(FillAttachment(dr));
                }
                dr.Close();
            }
            return al;
        }

        private static PermissionAttachment FillAttachment(IDataRecord dr)
        {           
            var result = new PermissionAttachment
                             {
                                 AttachmentId = Utilities.SafeConvertInt(dr["AttachId"]),
                                 UserId = Utilities.SafeConvertInt(dr["UserId"], -1),
                                 FileName = Utilities.SafeConvertString(dr["FileName"]),
                                 ContentType = Utilities.SafeConvertString(dr["ContentType"]),
                                 FileSize = Utilities.SafeConvertLong(dr["FileSize"]),
                                 FileId = dr["FileId"] as int?,
                                 AllowDownload = Utilities.SafeConvertBool(dr["AllowDownload"], true)
                             };

            if (dr.HasColumn("FileData"))
                result.FileData = dr["FileData"] as byte[];

            if (dr.HasColumn("CanRead"))
                result.CanRead = Utilities.SafeConvertString(dr["CanRead"], "0;1;-3;-1;|||"); // Default to public read permissions

            return result;
        }
	}
}

