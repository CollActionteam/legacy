import React, { useState } from "react";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Grid, FormGroup, TextField } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./ResetPassword.module.scss";
import { useLocation } from "react-router-dom";
import { Button } from "../../../components/Button/Button";
import Helmet from "react-helmet";
import { useFormik, Form, FormikProvider } from "formik";
import * as Yup from "yup";
import Loader from "../../../components/Loader/Loader";

const ResetPasswordPage = () => {
    const searchParams = new URLSearchParams(useLocation().search);
    const resetCode = searchParams.get("code");
    const email = searchParams.get("email");
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const [ resetPassword ] = useMutation(
        FORGOT_PASSWORD,
        {
            onCompleted: (data) => {
                if (data.user.resetPassword.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("Your password has been reset. You can log in with your new password now.");
                } else {
                    let error = data.user.resetPassword.errors.map((e: any) => e.description).join(", ");
                    setErrorMessage(error);
                    console.error(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
                console.error(data.message);
            }
        }
    );
    const formik = useFormik({
        initialValues: {
            password: "",
            confirmPassword: ""
        },
        validationSchema: Yup.object({
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
            })
        }),
        onSubmit: async (values) => {
            await resetPassword({
                variables: {
                    email: email,
                    code: resetCode,
                    password: values.password
                }});
            formik.setSubmitting(false);
        }
    });

    return <React.Fragment>
        <Helmet>
          <title>Reset Password</title>
          <meta name="description" content="Reset Password" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Reset Password</h1>
        </Section>
        <Alert type="error" text={errorMessage} />
        <Alert type="info" text={infoMessage} />
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <FormikProvider value={formik}>
                        <Form onSubmit={formik.handleSubmit}>
                            <FormGroup>
                                <TextField name="password" label="Password" type="password" error={(formik.touched.password || formik.submitCount > 0) && Boolean(formik.errors.password)} fullWidth { ...formik.getFieldProps('password') } />
                                { formik.touched.password || formik.submitCount > 0 ? <Alert type="error" text={formik.errors.password} /> : null }
                                <TextField name="confirmPassword" label="Confirm Password" error={(formik.touched.confirmPassword || formik.submitCount > 0) && Boolean(formik.errors.confirmPassword)} type="password" fullWidth { ...formik.getFieldProps('confirmPassword') } />
                                { formik.touched.confirmPassword || formik.submitCount > 0 ? <Alert type="error" text={formik.errors.confirmPassword} /> : null }
                            </FormGroup>

                            <div className={styles.submit}>
                                { formik.isSubmitting ? <Loader /> : <Button type="submit">Reset my password</Button> }
                            </div>
                        </Form>
                    </FormikProvider>
                </Grid>
            </Grid>
        </Section>
    </React.Fragment>;
};

export default ResetPasswordPage;

const FORGOT_PASSWORD = gql`
    mutation ResetPassword($email: String!, $code: String!, $password: String!) {
        user {
            resetPassword(email: $email, code: $code, password: $password) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }
`;