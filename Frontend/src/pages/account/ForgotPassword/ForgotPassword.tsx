import React, { useState } from "react";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Grid } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./ForgotPassword.module.scss";
import { Button } from "../../../components/Button/Button";
import { Helmet } from "react-helmet";
import { useFormik, FormikContext, Form, Field } from "formik";
import * as Yup from "yup";
import { TextField } from "formik-material-ui";

const ForgotPasswordPage = () => {

    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);

    const formik = useFormik({
        initialValues: {
            email: ''
        },
        validationSchema: Yup.object({
            email: Yup.string()
                .required("Please enter your e-mail address")
                .email("Please enter a valid e-mail address")
        }),
        validateOnChange: false,
        validateOnMount: false,
        validateOnBlur: true,
        onSubmit: (values, actions) => {
            forgotPassword();
            actions.setSubmitting(false);
        }
    });

    const [ forgotPassword ] = useMutation(
        FORGOT_PASSWORD,
        {
            variables: {
                email: formik.values.email
            },
            onCompleted: (data) => {
                if (data.user.forgotPassword.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("An e-mail has been sent, click on the link in that e-mail to reset your password");
                } else {
                    let error = data.user.forgotPassword.errors.map((e: any) => e.description).join(", ");
                    setErrorMessage(error);
                }
            },
            onError: (data) => {
                setErrorMessage(data.message);
                console.error(data.message);
            }
        }
    );

    return <>
        <Helmet>
            <title>Forgot Password CollAction</title>
            <meta name="description" content="Forgot Password" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Forgot Password</h1>
        </Section>
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <FormikContext.Provider value={formik}>
                        <Alert type="error" text={errorMessage} />
                        <Alert type="info" text={infoMessage} />

                        <Form>
                            <div className={styles.formRow}>
                                <Field
                                    name="email"                      
                                    label="E-mail"
                                    component={TextField}
                                    type="email"
                                    fullWidth
                                    >                      
                                </Field>
                            </div>
                            <div className={styles.formRow}>
                                <div className={styles.submit}>
                                    <Button type="submit">Reset my password</Button>
                                </div>
                            </div>
                        </Form>
                    </FormikContext.Provider>
                </Grid>
            </Grid>
        </Section>
    </>;
};

export default ForgotPasswordPage;

const FORGOT_PASSWORD = gql`
    mutation ForgotPassword($email: String!) {
        user {
            forgotPassword(email: $email) {
                succeeded
                errors {
                    code
                    description
                }
            }
        }
    }
`;