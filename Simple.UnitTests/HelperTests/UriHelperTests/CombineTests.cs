﻿using System;
using RafaelEstevam.Simple.Spider.Extensions;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.HelperTests.UriHelperTests
{
    public class CombineTests
    {
        [Theory]
        [InlineData("http://test.net/", "/apache", "http://test.net/apache")]
        [InlineData("http://test.net/", "apache", "http://test.net/apache")]
        [InlineData("http://test.net/", "apache/", "http://test.net/apache/")]
        public void UriHelper_CombineTests_Simple(string parent, string relative, string result)
        {
            string actual = UriExtensions.Combine(new Uri(parent), relative).ToString();
            Assert.Equal(result, actual);
        }

        [Theory]
        [InlineData("http://www.acme.com/support/intro.html", "suppliers.html", "http://www.acme.com/support/suppliers.html")]
        [InlineData("http://www.acme.com/support/intro.html", "../icons/logo.gif", "http://www.acme.com/icons/logo.gif")]
        public void UriHelper_CombineTests_WDhtml40_970917(string parent, string relative, string result)
        {
            //https://www.w3.org/TR/WD-html40-970917/htmlweb.html
            Assert.Equal(result, UriExtensions.Combine(new Uri(parent), relative).ToString());
        }
    }
}