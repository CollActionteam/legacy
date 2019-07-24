module.exports = {
  siteMetadata: {
    title: 'CollAction',
    menuLinks: [
      {
        name: 'Home',
        link: ''
      },
      {
         name:'Find Project',
         link:'/projects/find'
      },
      {
         name:'Start Project',
         link:'/projects/start'
      },
      {
         name:'About',
         link:'/about'
      }
    ]
  },
  plugins: [
    {
      resolve: "gatsby-source-graphql",
      options: {
        // This type will contain remote schema Query type
        typeName: "CollActionAPI",
        // This is the field under which it's accessible
        fieldName: "api",
        // URL to query from
        url: "https://collaction.org/v1/api", //TODO
      },
    },
    {
      resolve: `gatsby-plugin-sass`,
      options: {
        cssLoaderOptions: {
          camelCase: false,
        }
      }
    }
  ]
}
