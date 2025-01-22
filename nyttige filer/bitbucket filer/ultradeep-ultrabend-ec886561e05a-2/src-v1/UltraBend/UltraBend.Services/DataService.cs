using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Databases;
using UltraBend.Databases.DataSet;

namespace UltraBend.Services
{
    public class DataService : BaseFileService<DataSetDbContext>
    {
        public DataService(string  fileName, bool createFile = false)
        {
            FileName = fileName;

            if (createFile)
                using (new DataSetDbContext(fileName, createFile)) ;
        }

        public void AddData(string dataSetName, double[] data)
        {
            using (var context = new DataSetDbContext(FileName))
            {
                var dataSet = context.DataSets.FirstOrDefault(ds => ds.Name == dataSetName);
                if (dataSet == null)
                {
                    dataSet = new DataSet { Name = dataSetName };
                    context.DataSets.Add(dataSet);
                }

                context.DataItems.Add(new DataItem { Data = UltraBend.Common.Data.ByteSerialization.GetBytes(data), DataSet = dataSet });
                context.SaveChanges();
            }
            OnRepositoryUpdated(this);
        }

        public double[][] GetData(string dataSetName)
        {
            using (var context = new DataSetDbContext(FileName))
            {
                return context.DataItems.Where(di => di.DataSet.Name == dataSetName)
                    .Select(d => d.Data)
                    .AsEnumerable()
                    .Select(d => UltraBend.Common.Data.ByteSerialization.GetDoubles(d))
                    .ToArray();
            }
        }
    }
}
