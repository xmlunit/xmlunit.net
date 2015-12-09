using System;
using System.IO;
using System.Xml;
using NUnit.Framework.Constraints;
using Convert = Org.XmlUnit.Util.Convert;

namespace Org.XmlUnit.Constraints
{
    /// <summary>
    ///   Example Wrapper for {@link CompareConstraint}.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///    This example will write the Test-Input into the Files
    ///    System.  This could be useful for manual reviews or as
    ///    template for a control-File.
    ///   </para>
    /// </remarks>
    public class TestCompareConstraintWrapper : Constraint
    {
        private readonly CompareConstraint compareMatcher;
        private String fileName;
        internal TestCompareConstraintWrapper(CompareConstraint compareMatcher)
        {
            this.compareMatcher = compareMatcher;
        }
        public TestCompareConstraintWrapper WithTestFileName(string fileName)
        {
            this.fileName = fileName;
            return this;
        }

        public static TestCompareConstraintWrapper IsSimilarTo(object control)
        {
            return new TestCompareConstraintWrapper(CompareConstraint.IsSimilarTo(control));
        }

        public override bool Matches(object testItem)
        {
            if (fileName == null)
            {
                return compareMatcher.Matches(testItem);
            }
            // do something with your Test-Source
            var builder = Builder.Input.From(testItem);
            string testFile = WriteIntoTestResultFolder(builder.Build());
            return compareMatcher.Matches(Builder.Input.FromFile(testFile));
        }

        private string WriteIntoTestResultFolder(ISource source)
        {
            using (TextWriter fs = new StreamWriter(fileName))
            {
                marshal(source, fs);
            }
            return fileName;
        }

        private void marshal(ISource source, TextWriter fop)
        {
            XmlDocument doc = Convert.ToDocument(source);
            doc.WriteTo(new XmlTextWriter(fop));
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            compareMatcher.WriteDescriptionTo(writer);
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            compareMatcher.WriteMessageTo(writer);
        }
    }
}