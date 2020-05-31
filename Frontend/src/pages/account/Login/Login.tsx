import { FormControl, FormGroup, Grid } from "@material-ui/core";
import { Field, Form, FormikContext, useFormik } from "formik";
import { TextField } from "formik-material-ui";
import React, { useRef } from "react";
import { Helmet } from "react-helmet";
import { Link, useLocation } from "react-router-dom";
import * as Yup from "yup";
import { Alert } from "../../../components/Alert/Alert";
import { Button } from "../../../components/Button/Button";
import { Section } from "../../../components/Section/Section";
import { useSettings } from "../../../providers/SettingsProvider";
import styles from "./Login.module.scss";
import { useConsent, Consent } from "../../../providers/ConsentProvider";


const LoginPage = () => {
  const actionLogin = `${process.env.REACT_APP_BACKEND_URL}/account/login`;
  const actionExternalLogin = `${process.env.REACT_APP_BACKEND_URL}/account/externalLogin`;
  
  const errorUrl = `${window.location.origin}/account/login`;
  const searchParams = new URLSearchParams(useLocation().search);

  const returnUrlQuery = searchParams.get("returnUrl") ?? "";  
  const returnUrl = `${!returnUrlQuery.startsWith('http') ? window.location.origin : ""}${returnUrlQuery}`;
  const finishRegistrationSocialUrl = `${window.location.origin}/account/finish-registration-social`;
  
  const errorMessage = searchParams.get("message");

  const { externalLoginProviders } = useSettings();
  const { consent } = useConsent();

  const form = useRef(null) as any;
  const formik = useFormik({
    initialValues: {
      email: '',
      password: ''
    },
    validationSchema: Yup.object({
      email: Yup.string()
        .required("Please enter your e-mail address")
        .email("Please enter a valid e-mail address"),
      password: Yup.string()
        .required("Please enter your password")
    }),
    validateOnChange: false,
    validateOnMount: false,
    validateOnBlur: true,
    onSubmit: (values, actions) => {
      // Use a normal form post here
      actions.setSubmitting(false);
      form.current.submit();
    }
  });

  return (
    <>
      <Helmet>
        <title>Login</title>
        <meta name="description" content="Login" />
      </Helmet>
      <Section className={styles.intro}>
        <h1 className={styles.title}>Login</h1>
        <h2 className={styles.subtitle}>
          (Use a local account to log in )
        </h2>
      </Section>
      <Section color="grey">
        <Grid container justify="center">
          <Grid item sm={6}>
            <Alert type="error" text={errorMessage} />
            { consent.includes(Consent.Social) ?
              <>
                <form method="post" action={actionExternalLogin}>
                  <FormGroup>
                    <input type="hidden" name="returnUrl" value={returnUrl} />
                    <input type="hidden" name="finishRegistrationUrl" value={finishRegistrationSocialUrl} />
                    <input type="hidden" name="errorUrl" value={errorUrl} />
                    { externalLoginProviders.map(
                      (provider: string) => (
                        <FormControl key={provider} margin="normal">
                          <Button name="provider" value={provider}>
                            Login with {provider}
                          </Button>
                        </FormControl>
                      )
                    ) }
                  </FormGroup>
                </form>
                <div className={styles.divider}>
                  <span>or</span>
                </div>
              </> : 
              null 
            }
            <FormikContext.Provider value={formik}>
              <FormGroup>
                <Form className={styles.form} method="POST" action={actionLogin} ref={form}>
                  <input type="hidden" name="returnUrl" value={returnUrl} />
                  <input type="hidden" name="errorUrl" value={errorUrl} />
                  <div className={styles.formRow}>
                    <Field name="email"                      
                      label="E-mail"
                      component={TextField}
                      type="email"
                      fullWidth
                    / >                      
                  </div>
                  <div className={styles.formRow}>
                    <Field
                      name="password"
                      label="Password"
                      component={TextField}
                      type="password"
                      fullWidth
                    / >                      
                  </div>
                  <div className={styles.formRow}>
                    <div className={styles.submit}>
                      <Button type="submit">Login</Button>
                    </div>
                  </div>
                </Form>
                <Link to="/account/register-user">Register as new user</Link>
                <Link to="/account/forgot-password">I forgot my password</Link>
              </FormGroup>
            </FormikContext.Provider>

                {/* <FormControlLabel
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

                <br />

                <Link to="/account/register-user">Register as new user</Link>
                <Link to="/account/forgot-password">I forgot my password</Link>
              </FormGroup>
            </form> */}
          </Grid>
        </Grid>
      </Section>
  </>
  );
}

export default LoginPage;