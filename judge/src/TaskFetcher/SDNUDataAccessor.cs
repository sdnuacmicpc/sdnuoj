using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JudgeClient.Definition;
using JudgeClient.Fetcher;

namespace JudgeClient.SDNU
{
    public class SDNUDataAccessor : IDataAccessor
    {
        public int GetDataCount(Problem Problem)
        {
            try
            {
                return Directory.GetFiles(string.Format("{0}{1}\\input\\", _profile.TestDataSavePath, Problem.Id)).Count();
            }
            catch { }
            return 0;
        }

        public bool CheckValid(Problem Problem)
        {
            try
            {
                return File.ReadAllText(string.Format("{0}{1}\\valid_sign",
                    _profile.TestDataSavePath, Problem.Id)) == Problem.Version;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerator<TestData> GetDataEnumerator(Problem Problem)
        {
            var problem_path = _profile.TestDataSavePath + Problem.Id;
            var files = Directory.GetFiles(problem_path + "\\input\\");
            return new SDNUDataEnumerator((int index) => {
                if (index >= files.Length)
                    return null;
                var res = new TestData();
                res.Name = Path.GetFileNameWithoutExtension(files[index]);
                try { res.Input = File.ReadAllText(string.Format("{0}\\input\\{1}.in", problem_path, res.Name)); } catch { }
                try { res.Output = File.ReadAllText(string.Format("{0}\\output\\{1}.out", problem_path, res.Name)); } catch { }
                if (res.Input == null && res.Output == null)
                    return null;
                return res;
            });
        }

        public void Update(Problem Problem, IEnumerator<TestData> DataEnumerator)
        {
            var root = _profile.TestDataSavePath + Problem.Id;
            if (Directory.Exists(root))
                Directory.Delete(root, true);
            Directory.CreateDirectory(root);
            var input_path = string.Format("{0}\\input\\", root);
            if (!Directory.Exists(input_path))
                Directory.CreateDirectory(input_path);
            var output_path = string.Format("{0}\\output\\", root);
            if (!Directory.Exists(output_path))
                Directory.CreateDirectory(output_path);
            while (DataEnumerator.MoveNext())
            {
                File.WriteAllText(string.Format("{0}{1}.in", input_path, DataEnumerator.Current.Name), DataEnumerator.Current.Input);
                File.WriteAllText(string.Format("{0}{1}.out", output_path, DataEnumerator.Current.Name), DataEnumerator.Current.Output);
            }
            File.WriteAllText(string.Format("{0}{1}\\valid_sign", _profile.TestDataSavePath, Problem.Id), Problem.Version);
        }

        protected DataAccessorProfile _profile;
        void IModule.Configure(IProfile Profile)
        {
            try
            {
                _profile = Profile as DataAccessorProfile;
                if (!Directory.Exists(_profile.TestDataSavePath))
                    Directory.CreateDirectory(_profile.TestDataSavePath);
                int index = _profile.TestDataSavePath.LastIndexOfAny(new char[]{'/', '\\'});
                if (index == -1)
                    _profile.TestDataSavePath += '\\';
                else if (index < _profile.TestDataSavePath.Length - 1)
                    _profile.TestDataSavePath += _profile.TestDataSavePath[index];
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new CreateAndConfigureModuleException("Configure SDNUDataAccessor failed.", ex));
            }
        }
    }
}
