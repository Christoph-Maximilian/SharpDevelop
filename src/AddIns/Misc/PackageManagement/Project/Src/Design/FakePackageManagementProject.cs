﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementProject : IPackageManagementProject
	{
		public FakePackageManagementProject()
			: this("Test")
		{
		}
		
		public FakePackageManagementProject(string name)
		{
			FakeInstallPackageAction = new FakeInstallPackageAction(this);
			FakeUninstallPackageAction = new FakeUninstallPackageAction(this);
			
			this.Name = name;
		}
		
		public FakeInstallPackageAction FakeInstallPackageAction;
		public FakeUninstallPackageAction FakeUninstallPackageAction;
		
		public FakeUpdatePackageAction FirstFakeUpdatePackageActionCreated {
			get { return FakeUpdatePackageActionsCreated[0]; }
		}
		
		public FakeUpdatePackageAction SecondFakeUpdatePackageActionCreated {
			get { return FakeUpdatePackageActionsCreated[1]; }
		}
		
		public List<FakeUpdatePackageAction> FakeUpdatePackageActionsCreated = 
			new List<FakeUpdatePackageAction>();
		
		public string Name { get; set; }
		
		public bool IsPackageInstalled(string packageId)
		{
			return FakePackages.Any(p => p.Id == packageId);
		}
		
		public bool IsPackageInstalled(IPackage package)
		{
			return FakePackages.Contains(package);
		}
		
		public List<FakePackage> FakePackages = new List<FakePackage>();
		
		public IQueryable<IPackage> GetPackages()
		{
			return FakePackages.AsQueryable();
		}
		
		public List<FakePackageOperation> FakeInstallOperations = new List<FakePackageOperation>();
		public IPackage PackagePassedToGetInstallPackageOperations;
		public bool IgnoreDependenciesPassedToGetInstallPackageOperations;
		
		public virtual IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, bool ignoreDependencies)
		{
			PackagePassedToGetInstallPackageOperations = package;
			IgnoreDependenciesPassedToGetInstallPackageOperations = ignoreDependencies;
			
			return FakeInstallOperations;
		}
		
		public ILogger Logger { get; set; }
		
		public IPackage PackagePassedToInstallPackage;
		public IEnumerable<PackageOperation> PackageOperationsPassedToInstallPackage;
		public bool IgnoreDependenciesPassedToInstallPackage;
		
		public void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations, bool ignoreDependencies)
		{
			PackagePassedToInstallPackage = package;
			PackageOperationsPassedToInstallPackage = operations;
			IgnoreDependenciesPassedToInstallPackage = ignoreDependencies;
		}
		
		public FakePackageOperation AddFakeInstallOperation()
		{
			var package = new FakePackage("MyPackage");
			var operation = new FakePackageOperation(package, PackageAction.Install);
			FakeInstallOperations.Add(operation);
			return operation;
		}
		
		public FakePackageOperation AddFakeUninstallOperation()
		{
			var package = new FakePackage("MyPackage");
			var operation = new FakePackageOperation(package, PackageAction.Uninstall);
			FakeInstallOperations.Add(operation);
			return operation;			
		}
		
		public FakePackageRepository FakeSourceRepository = new FakePackageRepository();
		
		public IPackageRepository SourceRepository {
			get { return FakeSourceRepository; }
		}
		
		public IPackage PackagePassedToUninstallPackage;
		public bool ForceRemovePassedToUninstallPackage;
		public bool RemoveDependenciesPassedToUninstallPackage;
		
		public void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
		{
			PackagePassedToUninstallPackage = package;
			ForceRemovePassedToUninstallPackage = forceRemove;
			RemoveDependenciesPassedToUninstallPackage = removeDependencies;
		}
		
		public IPackage PackagePassedToUpdatePackage;
		public IEnumerable<PackageOperation> PackageOperationsPassedToUpdatePackage;
		public bool UpdateDependenciesPassedToUpdatePackage;
		public bool IsUpdatePackageCalled;
		
		public void UpdatePackage(IPackage package, IEnumerable<PackageOperation> operations, bool updateDependencies)
		{
			PackagePassedToUpdatePackage = package;
			PackageOperationsPassedToUpdatePackage = operations;
			UpdateDependenciesPassedToUpdatePackage = updateDependencies;
			IsUpdatePackageCalled = true;
		}
		
		public virtual InstallPackageAction CreateInstallPackageAction()
		{
			return FakeInstallPackageAction;
		}
		
		public virtual UninstallPackageAction CreateUninstallPackageAction()
		{
			return FakeUninstallPackageAction;
		}
				
		public UpdatePackageAction CreateUpdatePackageAction()
		{
			var action = new FakeUpdatePackageAction(this);
			FakeUpdatePackageActionsCreated.Add(action);
			return action;
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageInstalled;
		
		public void FirePackageInstalledEvent(PackageOperationEventArgs e)
		{
			if (PackageInstalled != null) {
				PackageInstalled(this, e);
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageUninstalled;
		
		public void FirePackageUninstalledEvent(PackageOperationEventArgs e)
		{
			if (PackageUninstalled != null) {
				PackageUninstalled(this, e);
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;
		
		public void FirePackageReferenceAddedEvent(PackageOperationEventArgs e)
		{
			if (PackageReferenceAdded != null) {
				PackageReferenceAdded(this, e);
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;
		
		public void FirePackageReferenceRemovedEvent(PackageOperationEventArgs e)
		{
			if (PackageReferenceRemoved != null) {
				PackageReferenceRemoved(this, e);
			}
		}
		
		public Project DTEProject;
		
		public Project ConvertToDTEProject()
		{
			return DTEProject;
		}
		
		public List<FakePackage> FakePackagesInReverseDependencyOrder = 
			new List<FakePackage>();
		
		public IEnumerable<IPackage> GetPackagesInReverseDependencyOrder()
		{
			return FakePackagesInReverseDependencyOrder;
		}
	}
}
