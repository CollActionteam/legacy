import { IUser, IProject } from "../../api/types";
import React, { useState } from "react";
import ProjectCard from "../ProjectCard/ProjectCard";
import { CardActions, CardContent, DialogContent, Dialog, DialogActions, FormGroup, TextField, Card } from "@material-ui/core";
import { Form, useFormik, FormikContext } from "formik";
import * as Yup from "yup";
import { Alert } from "../Alert/Alert";
import { useMutation, gql } from "@apollo/client";
import { Button } from "../Button/Button";

interface IProjectParticipatingProps {
    user: IUser;
    project: IProject;
};

const SEND_PROJECT_EMAIL = gql`
    mutation SendProjectEmail($projectId: ID!, $subject: String!, $message: String!) {
        project {
            sendProjectEmail(projectId: $projectId, subject: $subject, message: $message) {
                succeeded
                errors {
                    errorMessage
                }
                project {
                    id
                    canSendProjectEmail
                    numberProjectEmailsSent
                }
            }
        }
    }
`;

export default ({ user, project }: IProjectParticipatingProps) => {
    const [ showSendProject, setShowSendProject ] = useState(false);
    const [ error, setError ] = useState<string | null>(null);
    const [ info, setInfo ] = useState<string | null>(null);
    const [ sendEmail ] = useMutation(
        SEND_PROJECT_EMAIL,
        {
            onCompleted: (data) => {
                if (data.project.sendProjectEmail.succeeded) {
                    setInfo("Project E-Mail was sent");
                    setShowSendProject(false);
                } else {
                    const err = data.project.sendProjectEmail.errors.map((e: { errorMessage: string }) => e.errorMessage).join(", ");
                    setError(err);
                    console.error(err);
                }
            },
            onError: (data) => {
                setError(data.message);
                console.error(data.message);
                setShowSendProject(false);
            }
        }
    )
    const formik = useFormik({
        initialValues: {
            subject: "",
            message: ""
        },
        validationSchema: Yup.object({
            subject: Yup.string().required("E-Mail must have a subject"),
            message: Yup.string().required("E-Mail must have a message")
        }),
        onSubmit: (values) => {
            sendEmail(
                {
                    variables: {
                        projectId: project.id,
                        subject: values.subject,
                        message: values.message
                    }
                }
            );
        }
    });
    const projectEmailsLeft = 4 - project.numberProjectEmailsSent;
    const canSendUpTil = new Date(project.end);
    canSendUpTil.setDate(canSendUpTil.getDate() + 180);
    return <Card>
        <Alert type="error" text={error} />
        <Alert type="info" text={info} />
        <FormikContext.Provider value={formik}>
            <Form onSubmit={formik.handleSubmit}>
                <Dialog fullScreen open={showSendProject} onClose={() => setShowSendProject(false)}>
                    <DialogContent>
                        <h3>Send project e-mail for '{project.name}' project</h3>
                        <p>Hi {user.fullName ?? "project starter"}!</p>
                        <p>
                            You can send 4 e-mails in total (during and up to 180 days after the project ends). 
                            You have already sent { project.numberProjectEmailsSent } e-mails, and can still send { projectEmailsLeft } emails up until { canSendUpTil.toDateString() } 23:59 in the UTC (London) timezone. 
                            To personalize the message, you can add {'{firstname}'} and {'{lastname}'} to your message (including the brackets) - these will be substituted with the user's first and last name. 
                        </p>
                        <FormGroup>
                            <TextField fullWidth error={formik.submitCount > 0 && formik.errors.subject !== undefined} name="subject" label="Subject" {...formik.getFieldProps('subject')} />
                            { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.subject} /> : null }
                        </FormGroup>
                        <FormGroup>
                            <TextField fullWidth error={formik.submitCount > 0 && formik.errors.message !== undefined} multiline={true} name="message" rows={40} label="E-Mail Message" {...formik.getFieldProps('message')} />
                            { formik.submitCount > 0 ? <Alert type="error" text={formik.errors.message} /> : null }
                        </FormGroup>
                    </DialogContent>
                    <DialogActions>
                        <Button type="submit" onClick={() => formik.submitForm()}>Send E-Mail</Button>
                        <Button type="button" onClick={() => setShowSendProject(false)}>Close</Button>
                    </DialogActions>
                </Dialog>
            </Form>
        </FormikContext.Provider>
        <CardContent>
            <ProjectCard project={project} />
        </CardContent>
        <CardActions>
            { project.canSendProjectEmail ? <Button onClick={() => setShowSendProject(true)}>Send Project E-Mail</Button> : <Alert type="warning" text="You can't send project e-mails for this project currently. Project e-mails can be send from the moment the project has started up to 180 days after the project has ended. You can send 4 project e-mails in total. Contact hello@collaction.org if you think this is in error or if you want to raise the amount of e-mails you can send." /> }
        </CardActions>
    </Card>;
}