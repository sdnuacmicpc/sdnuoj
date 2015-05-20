using System;
using System.Collections.Generic;

namespace SDNUOJ.Configuration
{
    /// <summary>
    /// 关键字过滤器管理器
    /// </summary>
    public static class KeywordsFilterManager
    {
        #region 字段
        private static HashSet<String> _usernameKeywords = null;
        private static HashSet<String> _forumKeywords = null;
        #endregion

        #region 构造方法
        static KeywordsFilterManager()
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.UsernameKeywordsFilter))
            {
                _usernameKeywords = new HashSet<String>();

                String[] keywords = ConfigurationManager.UsernameKeywordsFilter.Split('|');

                for (Int32 i = 0; i < keywords.Length; i++)
                {
                    if (!String.IsNullOrEmpty(keywords[i]))
                    {
                        _usernameKeywords.Add(keywords[i].ToLower());
                    }
                }
            }

            if (!String.IsNullOrEmpty(ConfigurationManager.ForumKeywordsFilter))
            {
                _forumKeywords = new HashSet<String>();

                String[] keywords = ConfigurationManager.ForumKeywordsFilter.Split('|');

                for (Int32 i = 0; i < keywords.Length; i++)
                {
                    if (!String.IsNullOrEmpty(keywords[i]))
                    {
                        _forumKeywords.Add(keywords[i].ToLower());
                    }
                }
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 判断用户名昵称是否合法
        /// </summary>
        /// <param name="username">给定用户名</param>
        /// <returns>用户名是否合法</returns>
        public static Boolean IsUserNameLegal(String username)
        {
            if (_usernameKeywords == null || _usernameKeywords.Count < 1)
            {
                return true;
            }

            username = username.ToLower();

            foreach (String word in _usernameKeywords)
            {
                if (username.Contains(word))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断帖子是否合法
        /// </summary>
        /// <param name="username">给定用户名</param>
        /// <returns>用户名是否合法</returns>
        public static Boolean IsForumPostContentLegal(String content)
        {
            if (_forumKeywords == null || _forumKeywords.Count < 1)
            {
                return true;
            }

            content = content.ToLower();

            foreach (String word in _forumKeywords)
            {
                if (content.Contains(word))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}