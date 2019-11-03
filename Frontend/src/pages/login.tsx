import React from "react";
import { graphql, StaticQuery } from "gatsby";
import Layout from "../components/Layout";
import { Button } from "../components/Button";
import { Section } from "../components/Section";
import {
  Grid,
  TextField,
  FormControl,
  FormControlLabel,
  Checkbox,
  Box,
  FormGroup,
} from "@material-ui/core";
import styles from "./login.module.scss";

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
      const actionLogin = `${data.site.siteMetadata.backendUrl}/account/login`;
      const actionExternalLogin = `${data.site.siteMetadata.backendUrl}/account/externalLogin`;
      const returnUrl = data.site.siteMetadata.frontendUrl;
      const errorUrl = `${data.site.siteMetadata.frontendUrl}/error`;
      return (
        <Layout>
          <Section className={styles.intro}>
            <h1 className={styles.title}>Login</h1>
            <h2 className={styles.subtitle}>
              (Use a local account to log in )
            </h2>
          </Section>
          <Section color="grey">
            <Grid container justify="center">
              <Grid item sm={6}>
                <form method="post" action={actionExternalLogin}>
                  <FormGroup>
                    <input type="hidden" name="returnUrl" value={returnUrl} />
                    <input type="hidden" name="errorUrl" value={errorUrl} />
                    {data.site.siteMetadata.loginProviders.map(
                      (provider, index) => (
                        <FormControl key={index} margin="normal">
                          <Button name="provider" value={provider.name}>
                            Login with {provider.name}
                          </Button>
                        </FormControl>
                      )
                    )}
                  </FormGroup>
                </form>
                <div className={styles.divider}>
                  <span>or</span>
                </div>
                <form method="post" action={actionLogin}>
                  <FormGroup>
                    <FormControl margin="normal">
                      <TextField
                        className={styles.formControl}
                        label="E-mail"
                      />
                    </FormControl>
                    <FormControl margin="normal">
                      <TextField
                        className={styles.formControl}
                        label="Password"
                        type="password"
                      />
                    </FormControl>
                    <FormControlLabel
                      control={<Checkbox color="default" />}
                      label="Remember me"
                    />
                    <input type="hidden" name="returnUrl" value={returnUrl} />
                    <input type="hidden" name="errorUrl" value={errorUrl} />
                    <Button type="submit">Login</Button>
                  </FormGroup>
                </form>
              </Grid>
            </Grid>
          </Section>
        </Layout>
      );
    }}
  />
);
