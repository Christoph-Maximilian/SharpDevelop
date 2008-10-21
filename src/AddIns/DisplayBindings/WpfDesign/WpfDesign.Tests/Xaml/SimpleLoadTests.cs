﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3072 $</version>
// </file>

using System;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Xaml
{
	[TestFixture]
	public class SimpleLoadTests : TestHelper
	{
		[Test]
		public void Button()
		{
			TestLoading(@"
<Button
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
</Button>
			");
		}
		
		[Test]
		public void ButtonWithAttributes()
		{
			TestLoading(@"
<Button
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  Width=""300"" Height=""400"">
</Button>
			");
		}
		
		[Test]
		public void ButtonWithElementAttribute()
		{
			TestLoading(@"
<Button
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Button.Height>100</Button.Height>
</Button>
			");
		}
		
		[Test]
		public void ExampleClassTest()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassWithStringPropAttribute()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  StringProp=""a test string"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassWithEscapedBraceStringPropAttribute()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  StringProp=""{}{a test string}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultProperty()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   a test string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyExplicitly()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <t:ExampleClass.StringProp>
      a test string
   </t:ExampleClass.StringProp>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyBeforeOtherPropertyElement()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   a test string
   <t:ExampleClass.OtherProp>
      otherValue
   </t:ExampleClass.OtherProp>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyAfterOtherPropertyElement()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <t:ExampleClass.OtherProp>
      otherValue
   </t:ExampleClass.OtherProp>
   a test string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyBetweenOtherPropertyElement()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <t:ExampleClass.OtherProp>
      otherValue
   </t:ExampleClass.OtherProp>
   a test string
   <t:ExampleClass.OtherProp2>
      otherValue2
   </t:ExampleClass.OtherProp2>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void Container()
		{
			TestLoading(@"
<ExampleClassContainer
  xmlns=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <ExampleClassContainer.List>
      <ExampleClass OtherProp=""a""> </ExampleClass>
      <ExampleClass OtherProp=""b"" />
      <ExampleClass OtherProp=""c"" />
   </ExampleClassContainer.List>
</ExampleClassContainer>
			");
		}
		
		[Test]
		public void ContainerImplicitList()
		{
			TestLoading(@"
<ExampleClassContainer
  xmlns=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
      <ExampleClass OtherProp=""a"" />
      <ExampleClass OtherProp=""b"" />
</ExampleClassContainer>
			");
		}
		
		[Test]
		public void ExampleServiceTest()
		{
			TestLoading(@"
<t:ExampleDependencyObject
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  t:ExampleService.Example=""attached value"">
</t:ExampleDependencyObject>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithStringValue()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""a test string"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithTypeValue()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""{x:Type t:ExampleClass}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithTypeValue2()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""{x:Type TypeName=t:ExampleClass}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithNullValue()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""{x:Null}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithExplicitMarkupExtension()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <t:ExampleClass.ObjectProp>
    <x:Type TypeName=""t:ExampleClass""/>
  </t:ExampleClass.ObjectProp>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithExplicitMarkupExtension2()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + TestHelper.TestNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <t:ExampleClass.ObjectProp>
    <x:TypeExtension TypeName=""t:ExampleClass""/>
  </t:ExampleClass.ObjectProp>
</t:ExampleClass>
			");
		}
	}
}
