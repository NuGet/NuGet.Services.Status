// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;
using NuGet.Status.Utilities;

namespace NuGet.Status.Configuration
{
    public class NuGetGalleryVirtualPathProvider : VirtualPathProvider
    {
        public const string NuGetGalleryRelativePath = @"/NuGetGallery/src/NuGetGallery";

        public override bool FileExists(string virtualPath)
        {
            return FileExistsInternal(virtualPath) || Previous.FileExists(virtualPath);
        }

        private bool FileExistsInternal(string virtualPath)
        {
            return File.Exists(GetPhysicalPath(GetNuGetGalleryVirtualPath(virtualPath)));
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (Previous.FileExists(virtualPath))
            {
                return Previous.GetFile(virtualPath);
            }

            if (!FileExists(virtualPath))
            {
                return null;
            }

            var galleryVirtualPath = GetNuGetGalleryVirtualPath(virtualPath);
            return new NuGetGalleryVirtualFile(virtualPath, GetPhysicalPath(galleryVirtualPath));
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            try
            {
                if (virtualPathDependencies == null)
                {
                    return null;
                }

                var dependencyPhysicalPaths = new List<string>();
                foreach (string dependencyVirtualPath in virtualPathDependencies)
                {
                    string dependencyGalleryVirtualPath;
                    if (FileExistsInternal(dependencyVirtualPath))
                    {
                        dependencyGalleryVirtualPath = GetNuGetGalleryVirtualPath(dependencyVirtualPath);
                    }
                    else
                    {
                        dependencyGalleryVirtualPath = dependencyVirtualPath;
                    }

                    dependencyPhysicalPaths.Add(GetPhysicalPath(dependencyGalleryVirtualPath));
                }

                if (dependencyPhysicalPaths.Count == 0)
                {
                    return null;
                }

                return new CacheDependency(dependencyPhysicalPaths.ToArray(), utcStart);
            }
            catch (Exception e)
            {
                QuietLog.Log($"{nameof(NuGetGalleryVirtualPathProvider)}.{nameof(GetCacheDependency)}", "Failed to fetch cache dependency!", e);
                throw e;
            }
        }

        private static string GetPhysicalPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }

        private static string GetNuGetGalleryVirtualPath(string virtualPath)
        {
            if (virtualPath.Contains("~"))
            {
                return virtualPath.Replace("~", NuGetGalleryRelativePath);
            }

            return virtualPath.Insert(0, NuGetGalleryRelativePath);
        }
    }

    public class NuGetGalleryVirtualFile : VirtualFile
    {
        private string _physicalPath;

        public NuGetGalleryVirtualFile(string virtualPath, string physicalPath)
            : base(virtualPath)
        {
            _physicalPath = physicalPath;
        }

        public override Stream Open()
        {
            return new FileStream(_physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}