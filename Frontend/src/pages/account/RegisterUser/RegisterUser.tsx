import { gql, useMutation } from "@apollo/client";
import { FormControlLabel, Grid, FormHelperText, FormGroup } from "@material-ui/core";
import { Field, Form, FormikProvider, useFormik } from "formik";
import { Checkbox, TextField } from "formik-material-ui";
import React, { useState } from "react";
import { Helmet } from "react-helmet";
import { Link, useHistory } from "react-router-dom";
import * as Yup from "yup";
import { Alert } from "../../../components/Alert/Alert";
import { Button } from "../../../components/Button/Button";
import Loader from "../../../components/Loader/Loader";
import { Section } from "../../../components/Section/Section";
import styles from "./RegisterUser.module.scss";
import { useAnalytics } from "../../../providers/AnalyticsProvider";

const REGISTER_USER = gql`
    mutation RegisterUser($user: NewUserInputGraph!) {
        user {
            createUser(user: $user) {
                result {
                    succeeded
                    errors {
                        code
                        description
                    }
                }
            }
        }
    }
`;

const RegisterUserPage = () => {
    const history = useHistory();
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const { sendUserEvent } = useAnalytics();
    const [ createUser ] = useMutation(
        REGISTER_USER,
        {
            onCompleted: (data) => {
                if (data.user.createUser.result.succeeded) {
                    history.push("/account/register-user/complete");
                } else {
                    let error = data.user.createUser.result.errors.map((e: any) => e.description).join(", ");
                    setInfoMessage(null);
                    setErrorMessage(error);
                    formik.setSubmitting(false);
                    console.error(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
                formik.setSubmitting(false);
                console.error(data.message);
            }
        }
    );
    const formik = useFormik({
        initialValues: {
            firstName: "",
            lastName: "",
            email: "",
            confirmEmail: "",
            password: "",
            confirmPassword: "",
            isSubscribedNewsletter: false,
            privacyPolicy: false
        },
        validationSchema: Yup.object({
            firstName: Yup.string(),
            lastName: Yup.string(),
            email: Yup
                .string()
                .required("Please enter your e-mail address")
                .email("Please enter a valid e-mail address"),
            confirmEmail: Yup
                .string()
                .required("Please confirm your e-mail address")
                .when("email", {
                    is: val => val?.length > 0,
                    then: Yup
                        .string()
                        .oneOf([Yup.ref("email")], "Please confirm your e-mail address by entering the same one")
            }),
            password: Yup
                .string()
                .required("Please specify a password")
                .min(6, "Choose a safe password; make it at least 6 characters long"),
            confirmPassword: Yup
                .string()
                .required("Please confirm the password")
                .when("password", {
                    is: val => val?.length > 0,
                    then: Yup
                        .string()
                        .oneOf([Yup.ref("password")], "Please confirm the password by entering the same one")
            }),
            isSubscribedNewsletter: Yup.bool(),
            privacyPolicy: Yup.bool().oneOf([true], "Please agree to our privacy policy")
        }),
        onSubmit: async (values) => {
            await createUser({
                variables: {
                    user: {
                        firstName: values.firstName,
                        lastName: values.lastName,
                        email: values.email,
                        password: values.password,
                        isSubscribedNewsletter: values.isSubscribedNewsletter
                    }
                }
            });
            formik.setSubmitting(false);
        }
    });

    return <>
        <Helmet>
          <title>Register User</title>
          <meta name="description" content="Register User" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Create an Account</h1>
        </Section>
        <Section color="grey">
            <Grid container justify="center">
                <Grid item xs={12} sm={8} md={6}>
                    <Alert type="error" text={errorMessage} />
                    <Alert type="info" text={infoMessage} />
                    <FormikProvider value={formik}>
                        <Form onSubmit={formik.handleSubmit}>
                            <div className={styles.formRow}>
                                <Field
                                    name="firstName"
                                    label="First name"
                                    component={TextField}
                                    fullWidth
                                ></Field>
                            </div>
                            <div className={styles.formRow}>
                                <Field
                                    name="lastName"
                                    label="Last name"
                                    component={TextField}
                                    fullWidth
                                ></Field>
                            </div>
                            <div className={styles.formRow}>
                                <Field
                                    name="email"
                                    label="E-mail address"
                                    component={TextField}
                                    fullWidth
                                ></Field>
                            </div>
                            <div className={styles.formRow}>
                                <Field
                                    name="confirmEmail"
                                    label="Confirm e-mail address"
                                    component={TextField}
                                    fullWidth
                                ></Field>
                            </div>
                            <div className={styles.formRow}>
                                <Field
                                    name="password"
                                    label="Password"
                                    component={TextField}
                                    type="password"
                                    fullWidth
                                ></Field>
                            </div>
                            <div className={styles.formRow}>
                                <Field
                                    name="confirmPassword"
                                    label="Confirm your password"
                                    component={TextField}
                                    type="password"
                                    fullWidth
                                ></Field>
                            </div>
                            <div className={styles.formRow}>
                                <FormControlLabel
                                    control={
                                        <Field 
                                            name="isSubscribedNewsletter" 
                                            type="checkbox"
                                            color="primary"
                                            component={Checkbox}
                                        ></Field>}
                                    label={<span>I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! <span role="img" aria-label="smiley">🙂</span></span>}
                                />
                            </div>
                            <div className={styles.formRow}>
                                <FormGroup>
                                    <FormControlLabel
                                        control={
                                            <Field 
                                                name="privacyPolicy" 
                                                type="checkbox"
                                                color="primary"
                                                component={Checkbox}
                                            ></Field>}
                                        label={<span>I've read and agreed to the <Link to="/privacy-policy" target="_blank">privacy policy</Link></span>} />
                                    <FormHelperText error={true}>{formik.touched.privacyPolicy && formik.errors.privacyPolicy}</FormHelperText>
                                </FormGroup>
                            </div>
                            <div className={styles.formRow}>
                                <div className={styles.submit}>
                                    { formik.isSubmitting
                                        ? <Loader></Loader>
                                        : <Button type="submit" onClick={() => sendUserEvent(false, 'user', 'register', 'email', null)}>Register</Button>
                                    }
                                </div>
                            </div>
                        </Form>
                    </FormikProvider>
                </Grid>
            </Grid>
        </Section>
    </>;
};

export default RegisterUserPage;