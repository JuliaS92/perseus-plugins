﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PerseusApi.Utils;

namespace PerseusPluginLib.Test
{
    [TestFixture]
    public class PerseusUtilsTest
    {
        [Test]
        public void TestWriteMultiNumericColumnWithNulls()
        {
            var data = PerseusFactory.CreateDataWithAnnotationColumns();
            data.AddMultiNumericColumn("Test", "", new double[1][]);
            data.AddStringColumn("Test2", "", new string[1]);
            Assert.AreEqual(1, data.RowCount);
            var writer = new StreamWriter(new MemoryStream());
            PerseusUtils.WriteDataWithAnnotationColumns(data, writer);
        }
    }
}
