import React from "react";
import { graphql, StaticQuery } from "gatsby";

export default () => (
  <StaticQuery
    query={graphql`
      query BackendQuery {
        site {
          siteMetadata {
            backendUrl
            frontendUrl
            loginProviders {
              name
            }
          }
        }
      }
    `}
    render={data => {
      const actionLogin = data.site.siteMetadata.backendUrl + "/account/login";
      const actionExternalLogin =
        data.site.siteMetadata.backendUrl + "/account/externalLogin";
      const returnUrl = data.site.siteMetadata.frontendUrl;
      const errorUrl = data.site.siteMetadata.frontendUrl + "/error";
      return (
        <div>
          <h2>Login</h2>
          <form method="post" action={actionLogin}>
            <label>E-Mail</label>
            <input type="text" name="email" />
            <label>Password</label>
            <input type="password" name="password" />
            <label>Remember me</label>
            <input type="checkbox" name="rememberMe" />
            <input type="hidden" name="returnUrl" value={returnUrl} />
            <input type="hidden" name="errorUrl" value={errorUrl} />
            <input type="submit" value="Submit" />
          </form>
          <h2>External Login</h2>
          <form method="post" action={actionExternalLogin}>
            <label>Remember me</label>
            <input type="checkbox" name="rememberMe" />
            <input type="hidden" name="returnUrl" value={returnUrl} />
            <input type="hidden" name="errorUrl" value={errorUrl} />
            {data.site.siteMetadata.loginProviders.map((provider, index) => (
              <button name="provider" key={index} value={provider.name}>
                Login with {provider.name}
              </button>
            ))}
          </form>
        </div>
      );
    }}
  />
);
