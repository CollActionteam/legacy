import React, { useState } from "react";
import { Section } from "../../../components/Section/Section";
import { Alert } from "../../../components/Alert/Alert";
import { Grid, Checkbox, FormGroup, TextField, Button, FormControlLabel, FormControl } from "@material-ui/core";
import { gql, useMutation } from "@apollo/client";
import styles from "./FinishRegistration.module.scss";
import { Link, useLocation } from "react-router-dom";
import { useFormik, FormikProvider, Form } from "formik";
import * as Yup from "yup";

const FINISH_REGISTRATION = gql`
    mutation FinishRegistration($user: NewUserInputGraph!, $code: String!) {
        user {
            finishRegistration(user: $user, code: $code) {
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

const FinishRegistrationPage = () => {
    const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
    const [ infoMessage, setInfoMessage ] = useState<string | null>(null);
    const searchParams = new URLSearchParams(useLocation().search);
    const email = searchParams.get("email");
    const code = searchParams.get("code");
    const [ finishRegistration ] = useMutation(
        FINISH_REGISTRATION,
        {
            onCompleted: (data) => {
                if (data.user.finishRegistration.result.succeeded) {
                    setErrorMessage(null);
                    setInfoMessage("You have been registered. You can now login with your new account.");
                } else {
                    let error = data.user.finishRegistration.result.errors.map((e: any) => e.description).join(", ");
                    setInfoMessage(null);
                    setErrorMessage(error);
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
            firstName: "",
            lastName: "",
            password: "",
            confirmPassword: "",
            isSubscribedNewsletter: false,
            privacyPolicy: false
        },
        validationSchema: Yup.object({
            firstName: Yup.string(),
            lastName: Yup.string(),
            password: Yup.string().min(6, "Password must be at least six characters long")
                                  .required("Must fill in a password at least six characters long"),
            confirmPassword: Yup.string().required("Must fill in a confirmation password")
                                         .when("password", {
                is: val => val && val.length > 0,
                then: Yup.string()
                         .oneOf([Yup.ref("password")], "Both passwords need to be the same")
                         .required("Must fill in a confirmation password")
            }),
            isSubscribedNewsletter: Yup.bool(),
            privacyPolicy: Yup.bool().oneOf([true], "You must agree to our privacy policy")
        }),
        onSubmit: (values) => {
            finishRegistration({
                variables: {
                    user: {
                        firstName: values.firstName,
                        lastName: values.lastName,
                        email: email,
                        password: values.password,
                        isSubscribedNewsletter: values.isSubscribedNewsletter
                    },
                    code: code
                }
            });
        }
    });

    return <React.Fragment>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Finish User Registration</h1>
        </Section>
        <Alert type="error" text={errorMessage} />
        <Alert type="info" text={infoMessage} />
        <Section color="grey">
            <Grid container justify="center">
                <Grid item sm={6}>
                    <FormikProvider value={formik}>
                        <Form onSubmit={formik.handleSubmit}>
                            <FormGroup>
                                <TextField name="firstName" label="First Name" type="text" { ... formik.getFieldProps('firstName') } />
                                <TextField name="lastName" label="Last Name" type="text" { ...formik.getFieldProps('lastName') } />
                                <TextField name="password" label="Password" type="password" error={formik.touched.password && formik.errors.password !== undefined} { ...formik.getFieldProps('password') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.password} /> : null }
                                <TextField name="confirmPassword" label="Confirm Password" type="password" error={formik.touched.confirmPassword && formik.errors.confirmPassword !== undefined} { ...formik.getFieldProps('confirmPassword') } />
                                { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.confirmPassword} /> : null }
                                <FormControlLabel
                                    control={<Checkbox name="isSubscribedNewsletter" { ...formik.getFieldProps('isSubscribedNewsletter')} />}
                                    label={<React.Fragment>I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! <span role="img" aria-label="smiley">🙂</span></React.Fragment>} />
                                <FormControl>
                                    <FormControlLabel
                                        control={<Checkbox name="privacyPolicy" { ...formik.getFieldProps('privacyPolicy') } />}
                                        label={<React.Fragment>I've read and agreed to our <Link to="/privacy-policy">privacy policy</Link></React.Fragment>}
                                        />
                                    { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.privacyPolicy} /> : null }
                                </FormControl>
                                <Button type="submit">Register</Button>
                            </FormGroup>
                        </Form>
                    </FormikProvider>
                </Grid>
            </Grid>
        </Section>
    </React.Fragment>;
};

export default FinishRegistrationPage;