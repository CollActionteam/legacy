import React from "react";
import { graphql } from "gatsby";

export default ({ data }) => {
    let actionLogin = data.site.siteMetadata.backendUrl + "/account/login";
    let actionExternalLogin = data.site.siteMetadata.backendUrl + "/account/externalLogin";
    let returnUrl = data.site.siteMetadata.frontendUrl;
    let errorUrl = data.site.siteMetadata.frontendUrl + "error";
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
                {data.site.siteMetadata.loginProviders.map(provider => <button name="provider" value={provider.name}>Login with {provider.name}</button>)}
            </form>
        </div>);
};

export const query = graphql`
    query BackendQuery {
        site {
            siteMetadata {
                backendUrl,
                frontendUrl,
                loginProviders {
                    name
                }
            }
        }
    }
`;