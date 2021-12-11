using cymaxdemo.businesslogic;
using cymaxdemo.controller;
using cymaxdemo.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xunit;

namespace unittestcymax
{
    public class UnitTest1
    {
        private readonly cymaxController _controller;
        public UnitTest1()
        {
            CallAPI call = new CallAPI(default);
            this._controller = new cymaxController(call);
        }
        [Fact]
        public void checksourcenull()
        {
            var okResult = _controller.getbestresult(generatefakedata_source());
            Assert.IsType<BadRequestObjectResult>(okResult.Result);
        }
        [Fact]
        public void checkdestinationenull()
        {
            var okResult = _controller.getbestresult(generatefakedata_destination());
            Assert.IsType<BadRequestObjectResult>(okResult.Result);
        }
        [Fact]
        public void checkdimension()
        {
            var okResult = _controller.getbestresult(generatefakedata_dimension());
            Assert.IsType<BadRequestObjectResult>(okResult.Result);
        }


        public getBestResultModel generatefakedata_source()
        {
            getBestResultModel data = new getBestResultModel();
            data.point_source = null;
            data.point_destination = "BC";
            data.dimension = new List<dimension>() {
                new dimension(){length=1,height=2,width=3 },
                new dimension(){length=3,height=2,width=1 },
                new dimension(){length=11,height=12,width=2 },
                };
            return data;
        }
        public getBestResultModel generatefakedata_destination()
        {
            getBestResultModel data = new getBestResultModel();
            data.point_source = "winnipeg";
            data.point_destination = null;
            data.dimension = new List<dimension>() {
                new dimension(){length=1,height=2,width=3 },
                new dimension(){length=3,height=2,width=1 },
                new dimension(){length=11,height=12,width=2 },
                };
            return data;
        }
        public getBestResultModel generatefakedata_dimension()
        {
            getBestResultModel data = new getBestResultModel();
            data.point_source = "winnipeg";
            data.point_destination = "BC";
            data.dimension = null;
            return data;
        }
    }
}
