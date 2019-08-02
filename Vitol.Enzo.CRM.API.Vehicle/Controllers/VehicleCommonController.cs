using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.Application;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.Vehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleCommonController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// CentreController initializes class object.
        /// </summary>
        /// <param name="vehicleCommonApplication"></param>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public VehicleCommonController(IVehicleCommonApplication vehicleCommonApplication)
        {
            this.VehicleCommonApplication = vehicleCommonApplication;
        }
        #endregion
        #region Properties and Data Members
        public IVehicleCommonApplication VehicleCommonApplication { get; }

        #endregion

        #region API Methods

        #region TrimLevel

        [HttpPost]
        [Route("addTrimLevel")]
        public async Task<IActionResult> Add([FromBody] TrimLevel trimLevel)
        {
            string newCRMtrimLevelId = string.Empty;
            newCRMtrimLevelId = await this.VehicleCommonApplication.AddTrimLevel(trimLevel);
            return Ok(newCRMtrimLevelId);
        }

        [HttpPut]
        [Route("updateTrimLevel")]
        public async Task<bool> Update([FromBody] TrimLevel trimLevel)
        {
            return await this.VehicleCommonApplication.UpdateTrimLevel(trimLevel);
        }
        #endregion

        #region BodyType

        [HttpPost]
        [Route("addBodyType")]
        public async Task<IActionResult> Add([FromBody] BodyType bodyType)
        {
            string newCRMBodyTypeId = string.Empty;
            newCRMBodyTypeId = await this.VehicleCommonApplication.AddBodyType(bodyType);
            return Ok(newCRMBodyTypeId);
        }

        [HttpPut]
        [Route("updateBodyType")]
        public async Task<bool> UpdateBodyType([FromBody] BodyType bodyType)
        {
            return await this.VehicleCommonApplication.UpdateBodyType(bodyType);
        }
        #endregion

        #region TransmissionType

        [HttpPost]
        [Route("addTransmissionType")]
        public async Task<IActionResult> AddTransmissionType([FromBody] TransmissionType transmissionType)
        {
            string newCRMTransmissionTypeId = string.Empty;
            newCRMTransmissionTypeId = await this.VehicleCommonApplication.AddTransmissionType(transmissionType);
            return Ok(newCRMTransmissionTypeId);
        }

        [HttpPut]
        [Route("updateTransmissionType")]
        public async Task<bool> UpdateTransmissionType([FromBody] TransmissionType transmissionType)
        {
            return await this.VehicleCommonApplication.UpdateTransmissionType(transmissionType);
        }
        #endregion

        #region FuelType

        [HttpPost]
        [Route("addFuelType")]
        public async Task<IActionResult> AddFuelType([FromBody] FuelType fuelType)
        {
            string newCRMFuelTypeId = string.Empty;
            newCRMFuelTypeId = await this.VehicleCommonApplication.AddFuelType(fuelType);
            return Ok(newCRMFuelTypeId);
        }

        [HttpPut]
        [Route("updateFuelType")]
        public async Task<bool> UpdateFuelType([FromBody] FuelType fuelType)
        {
            return await this.VehicleCommonApplication.UpdateFuelType(fuelType);
        }
        #endregion

        #region EngineType

        [HttpPost]
        [Route("addEngineType")]
        public async Task<IActionResult> AddEngineType([FromBody] EngineType engineType)
        {
            string newEngineTypeId = string.Empty;
            newEngineTypeId = await this.VehicleCommonApplication.AddEngineType(engineType);
            return Ok(newEngineTypeId);
        }

        [HttpPut]
        [Route("updateEngineType")]
        public async Task<bool> UpdateEngineType([FromBody] EngineType engineType)
        {
            return await this.VehicleCommonApplication.UpdateEngineType(engineType);
        }
        #endregion

        #region DriveType

        [HttpPost]
        [Route("addDriveType")]
        public async Task<IActionResult> AddDriveType([FromBody] DriveType driveType)
        {
            string newCRMDriveTypeId = string.Empty;
            newCRMDriveTypeId = await this.VehicleCommonApplication.AddDriveType(driveType);
            return Ok(newCRMDriveTypeId);
        }

        [HttpPut]
        [Route("updateDriveType")]
        public async Task<bool> UpdateDriveType([FromBody] DriveType driveType)
        {
            return await this.VehicleCommonApplication.UpdateDriveType(driveType);
        }
       
        #endregion

        #region Facelift

        [HttpPost]
        [Route("addFacelift")]
        public async Task<IActionResult> AddFacelift([FromBody] Facelift facelift)
        {
            string newCRMfaceliftId = string.Empty;
            newCRMfaceliftId = await this.VehicleCommonApplication.AddFaceLift(facelift);
            return Ok(newCRMfaceliftId);
        }

        [HttpPut]
        [Route("updateFacelift")]
        public async Task<bool> UpdateFuelType([FromBody] Facelift facelift)
        {
            return await this.VehicleCommonApplication.UpdateFaceLift(facelift);
        }
        #endregion

        #region Color

        [HttpPost]
        [Route("addColor")]
        public async Task<IActionResult> AddColor([FromBody] Color color)
        {
            string newCRMColorId = string.Empty;
            newCRMColorId = await this.VehicleCommonApplication.AddColor(color);
            return Ok(newCRMColorId);
        }

        [HttpPut]
        [Route("updateColor")]
        public async Task<bool> UpdateColor([FromBody] Color color)
        {
            return await this.VehicleCommonApplication.UpdateColor(color);
        }
        #endregion
        
        #endregion
    }
}