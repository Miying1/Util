using System.Collections.Generic;
using Util.Helpers;
using Util.ObjectMapping.AutoMapper.Tests.Infrastructure;
using Util.ObjectMapping.AutoMapper.Tests.Samples;
using Xunit;

namespace Util.ObjectMapping.AutoMapper.Tests {
    /// <summary>
    /// ����ӳ��������
    /// </summary>
    public class ObjectMapperExtensionsTest : TestBase {
        /// <summary>
        /// ����ӳ�� - Sample -> Sample2 ����������������ӳ���ϵ
        /// </summary>
        [Fact]
        public void TestMapTo_1() {
            var sample = new Sample { StringValue = "a" };
            var sample2 = sample.MapTo<Sample2>();
            Assert.Equal( "a", sample2.StringValue );
        }

        /// <summary>
        /// ����ӳ��- ����������������ӳ���ϵ - ����������
        /// </summary>
        [Fact]
        public void TestMapTo_2() {
            var sample = new Sample { StringValue = "a" };
            var sample2 = new Sample2();
            sample.MapTo( sample2 );
            Assert.Equal( "a", sample2.StringValue );
        }

        /// <summary>
        /// ����ӳ�� - Sample2 -> Sample δ��������������,���Զ�����ӳ��
        /// </summary>
        [Fact]
        public void TestMapTo_3() {
            var sample2 = new Sample2 { StringValue = "a" };
            var sample = sample2.MapTo<Sample>();
            Assert.Equal( "a", sample.StringValue );
        }

        /// <summary>
        /// ����ӳ�� - ӳ����ͬ�������Ĳ�ͬ����
        /// </summary>
        [Fact]
        public void TestMapTo_4() {
            Sample sample = new Sample { Test3 = new Sample3Copy { StringValue = "a" } };
            Sample2 sample2 = sample.MapTo<Sample2>();
            Assert.Equal( "a", sample2.Test3.StringValue );
        }

        /// <summary>
        /// ����ӳ�� - ӳ����ͬ�������Ĳ�ͬ���󼯺�
        /// </summary>
        [Fact]
        public void TestMapTo_5() {
            Sample sample = new Sample { TestList = new List<Sample3Copy> { new() { StringValue = "a" }, new() { StringValue = "b" } } };
            Sample2 sample2 = sample.MapTo<Sample2>();
            Assert.Equal( 2, sample2.TestList.Count );
            Assert.Equal( "a", sample2.TestList[0].StringValue );
            Assert.Equal( "b", sample2.TestList[1].StringValue );
        }

        /// <summary>
        /// ����ӳ�伯��
        /// </summary>
        [Fact]
        public void TestMapTo_6() {
            List<Sample> sampleList = new List<Sample> { new() { StringValue = "a" }, new() { StringValue = "b" } };
            List<Sample2> sample2List = new List<Sample2>();
            sampleList.MapTo( sample2List );
            Assert.Equal( 2, sample2List.Count );
            Assert.Equal( "a", sample2List[0].StringValue );
        }

        /// <summary>
        /// ����ӳ�伯��
        /// </summary>
        [Fact]
        public void TestMapTo_7() {
            List<Sample> sampleList = new List<Sample> { new() { StringValue = "a" }, new() { StringValue = "b" } };
            List<Sample2> sample2List = sampleList.MapTo<List<Sample2>>();
            Assert.Equal( 2, sample2List.Count );
            Assert.Equal( "a", sample2List[0].StringValue );
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Fact]
        public void TestMapTo_8() {
            Thread.ParallelFor( () => {
                var sample = new Sample { StringValue = "a" };
                var sample2 = sample.MapTo<Sample2>();
                Assert.Equal( "a", sample2.StringValue );
            }, 20 );
        }

        /// <summary>
        /// ����ӳ�伯��
        /// </summary>
        [Fact]
        public void TestMapToList() {
            List<Sample> sampleList = new List<Sample> { new() { StringValue = "a" }, new() { StringValue = "b" } };
            List<Sample2> sample2List = sampleList.MapToList<Sample2>();
            Assert.Equal( 2, sample2List.Count );
            Assert.Equal( "a", sample2List[0].StringValue );
        }

        /// <summary>
        /// ӳ�伯�� - ���Կռ���
        /// </summary>
        [Fact]
        public void TestMapToList_2() {
            List<Sample> sampleList = new List<Sample>();
            List<Sample2> sample2List = sampleList.MapToList<Sample2>();
            Assert.Empty( sample2List );
        }

        /// <summary>
        /// ӳ�伯�� - ��������
        /// </summary>
        [Fact]
        public void TestMapToList_3() {
            Sample[] sampleList = { new() { StringValue = "a" }, new() { StringValue = "b" } };
            List<Sample2> sample2List = sampleList.MapToList<Sample2>();
            Assert.Equal( 2, sample2List.Count );
            Assert.Equal( "a", sample2List[0].StringValue );
        }
    }
}
