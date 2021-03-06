﻿using System;
using System.CodeDom;
using System.Collections.Generic;

namespace ExcelReaderConsole.Models
{
    public class DocumentsStorage
    {
        public enum FilesType
        {
            Text = 0,
            ScanCopy,
            TextPdf,
            Attachments
        }

        private int attributeCount = 0;
        private List<DocumentAttribute> attributes;
        private Dictionary<string, int> mapAttributIdToNumberIndexList;
        private List<string> usedAttributeIds;
        private Dictionary<string, Document> documentsDictionary;

        public DocumentsStorage()
        {
            documentsDictionary = new Dictionary<string, Document>();
            usedAttributeIds = new List<string>();
        }

        public void Clean()
        {
            attributes = null;
            mapAttributIdToNumberIndexList = null;
            usedAttributeIds = new List<string>();
            documentsDictionary = new Dictionary<string, Document>();
        }

        public void Init(int attributeCount)
        {
            Clean();
            this.attributeCount = attributeCount;
            attributes = new List<DocumentAttribute>(attributeCount);
            mapAttributIdToNumberIndexList = new Dictionary<string, int>(attributeCount);
            for (int i = 0; i < attributeCount; i++)
            {
                attributes.Add(new DocumentAttribute());
            }
        }

        public void SetAttributeName(int number, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("name couldn't be null or empty");
            attributes[number].Name = name;
        }

        public void SetAttributeIdentifier(int number, string identifier)
        {
            if (number > attributeCount) throw new IndexOutOfRangeException($"number is {number}, but it should be less than {attributeCount}");
            if (string.IsNullOrEmpty(identifier)) throw new Exception("idntifier couldn't be null or empty");
            attributes[number].Identifier = identifier;
            mapAttributIdToNumberIndexList.Add(identifier, number);
        }

        public string GetAttributeIdentifier(int number)
        {
            if (number > attributeCount) throw new IndexOutOfRangeException($"number is {number}, but it should be less than {attributeCount}");
            return attributes[number].Identifier;
        }

        public void SetAttributeValue(string documentId, string attributeId, DocumentAttributeValue value)
        {
            if (!documentsDictionary.ContainsKey(documentId)) throw new Exception($"Incorrect documentId: {documentId}");
            if (string.IsNullOrEmpty(attributeId)) throw new Exception("attributeId can't be null or empty");
            if (value == null) throw new Exception("document attribute value can't be null");

            documentsDictionary[documentId].SetAttributeValue(attributeId, value);
            if (!usedAttributeIds.Contains(attributeId))
            {
                usedAttributeIds.Add(attributeId);
            }
        }

        public string AddDocument(string identifier = null)
        {
            string documentIdentifier = string.IsNullOrEmpty(identifier)
                ? string.Format("{0:D5}", documentsDictionary.Count + 1)
                : identifier;
            Document newDocument = new Document(documentIdentifier, attributes);
            documentsDictionary.Add(newDocument.Identifier, newDocument);
            return newDocument.Identifier;
        }

        private void AddUsedAttributes(Document document)
        {
            foreach (var attr in document.GetNotNullAttributes())
            {
                if (!usedAttributeIds.Contains(attr.Key))
                {
                    usedAttributeIds.Add(attr.Key);
                }
            }
        }

        public string AddDocument(Document document)
        {
            if (document != null)
            {
                documentsDictionary.Add(document.Identifier, document);
                AddUsedAttributes(document);
                return document.Identifier;
            }
            throw new ArgumentException("[DocumentStorage] [AddDocument] Document can't be null ");
        }

        public Document CreateDocument(string identifier = null)
        {
            try
            {
                if (identifier != null)
                {
                    string[] parts = identifier.Split(':');
                    if (parts.Length == 2)
                    {
                        return new DocumentEdition(parts[1], parts[0], attributes);
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }

            string documentIdentifier = string.IsNullOrEmpty(identifier)
                ? string.Format("{0:D5}", documentsDictionary.Count + 1)
                : identifier;
            return new Document(documentIdentifier, attributes);
        }

        public void SetTextFileName(string documentId, string textFileName)
        {
            if (!documentsDictionary.ContainsKey(documentId))
            {
                throw new Exception($"[DocumentStorage] [SetTextFileName] Incorrect documentId: {documentId}");
            }
            documentsDictionary[documentId].TextFileName = textFileName;
        }

        public void SetScanFileName(string documentId, string scanFileName)
        {
            if (!documentsDictionary.ContainsKey(documentId))
            {
                throw new Exception($"[DocumentStorage] [SetScanFileName] Incorrect documentId: {documentId}");
            }
            documentsDictionary[documentId].ScanFileName = scanFileName;
        }

        public void SetTextPdfFileName(string documentId, string textPdfFilesNames)
        {
            if (!documentsDictionary.ContainsKey(documentId))
            {
                throw new Exception($"[DocumentStorage] [SetTextPdfFileName] Incorrect documentId: {documentId}");
            }
            documentsDictionary[documentId].TextPdfFileName = textPdfFilesNames;
        }

        public void SetAttachmentsFilesName(string documentId, string attachmentsFilesNames)
        {
            if (!documentsDictionary.ContainsKey(documentId))
            {
                throw new Exception($"[DocumentStorage] [SetAttachmentsFilesName] Incorrect documentId: {documentId}");
            }
            documentsDictionary[documentId].AttachmentsFilesNames = attachmentsFilesNames;
        }

        public IEnumerable<Document> GetDocuments()
        {
            foreach (var value in documentsDictionary.Values)
            {
                yield return value;
            }
        }

        public Document GetDocument(string identifyer)
        {
            if (documentsDictionary.ContainsKey(identifyer))
            {
                return documentsDictionary[identifyer];
            }
            return null;
        }

        public IEnumerable<DocumentAttribute> Attributes
        {
            get { return attributes; }
        }

        public IEnumerable<DocumentAttribute> GetUsedDocumentAttributes()
        {
            foreach (var usedAttributeId in usedAttributeIds)
            {
                if (mapAttributIdToNumberIndexList.ContainsKey(usedAttributeId))
                {
                    int index = mapAttributIdToNumberIndexList[usedAttributeId];
                    yield return attributes[index];
                }
            }
        }
    }
}
