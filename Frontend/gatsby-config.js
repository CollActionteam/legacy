module.exports = {
  siteMetadata: {
    title: "CollAction",
    author: "CollAction",
    description: "CollAction",
    backendUrl: "https://localhost:44301",
    frontendUrl: "http://localhost:8000",
    loginProviders: [ 
      {
        name: "Google"
      },
      { 
        name: "Twitter"
      },
      {
         name: "Facebook"
      }
    ],
    menuLinks: [
      {
        name: "Home",
        link: "",
      },
      {
        name: "Find Project",
        link: "/projects/find",
      },
      {
        name: "Start Project",
        link: "/projects/start",
      },
      {
        name: "Blog",
        link: "/blog",
      },
      {
        name: "About",
        link: "/about",
      },
      {
        name: "Login",
        link: "/login",
      },
    ],
  },
  plugins: [
    {
      resolve: 'gatsby-plugin-sass',
      options: {
        cssLoaderOptions: {
          camelCase: false,
        },
      },
    },
    {
      resolve: 'gatsby-source-filesystem',
      options: {
        path: `${__dirname}/src/content/blog`,
        name: 'blog',
      },
    },
    {
      resolve: 'gatsby-source-filesystem',
      options: {
        path: `${__dirname}/src/content/homepage`,
        name: 'homepage',
      },
    },
    {
      resolve: 'gatsby-source-filesystem',
      options: {
        path: `${__dirname}/static/assets`,
        name: 'assets',
      },
    },
    {
      resolve: 'gatsby-transformer-remark',
      options: {
        plugins: [
          {
            resolve: 'gatsby-remark-images',
            options: {
              maxWidth: 590,
            },
          },
          {
            resolve: 'gatsby-remark-responsive-iframe',
            options: {
              wrapperStyle: 'margin-bottom: 1.0725rem',
            },
          },
          'gatsby-remark-prismjs',
          'gatsby-remark-copy-linked-files',
          'gatsby-remark-smartypants',
        ],
      },
    },
    'gatsby-transformer-sharp',
    'gatsby-plugin-sharp',
    'gatsby-plugin-offline',
    'gatsby-plugin-react-helmet',
    'gatsby-plugin-netlify-cms'
  ]
};