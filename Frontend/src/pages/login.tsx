import React from "react";
import { graphql, StaticQuery } from "gatsby";
import Layout from "../components/Layout";
import { Button } from "../components/Button/Button";
import { Section } from "../components/Section";
import { Location } from "@reach/router";
import {
  Grid,
  TextField,
  FormControl,
  FormControlLabel,
  Checkbox,
  FormGroup,
} from "@material-ui/core";
import styles from "./login.module.scss";

export default () => (
  <Location>
    {({ location }) => (
      <StaticQuery
        query={graphql`
          query BackendQuery {
            site {
              siteMetadata {
                backendUrl
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
          const returnUrl = location.href;
          const errorUrl = `${returnUrl}/error`;
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
                        <input
                          type="hidden"
                          name="returnUrl"
                          value={returnUrl}
                        />
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
                            name="Email"
                            className={styles.formControl}
                            label="E-mail"
                          />
                        </FormControl>
                        <FormControl margin="normal">
                          <TextField
                            name="Password"
                            className={styles.formControl}
                            label="Password"
                            type="password"
                          />
                        </FormControl>
                        <FormControlLabel
                          control={<Checkbox color="default" />}
                          label="Remember me"
                        />
                        <input
                          type="hidden"
                          name="returnUrl"
                          value={returnUrl}
                        />
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
    )}
  </Location>
);
