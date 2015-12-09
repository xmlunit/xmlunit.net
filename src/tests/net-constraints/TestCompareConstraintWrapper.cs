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

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (fileName == null)
            {
                return compareMatcher.ApplyTo(actual);
            }
            // do something with your Test-Source
            var builder = XmlUnit.Builder.Input.From(actual);
            string testFile = WriteIntoTestResultFolder(builder.Build());
            return compareMatcher.ApplyTo(XmlUnit.Builder.Input.FromFile(testFile));
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
    }
}