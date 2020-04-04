import { IUser, IProject } from "../../api/types";
import React, { useState } from "react";
import ProjectCard from "../ProjectCard/ProjectCard";
import { Card, CardActions, CardContent, Button, DialogContent, Dialog, DialogActions } from "@material-ui/core";
import { Form, useFormik, FormikContext } from "formik";

interface IProjectParticipatingProps {
    user: IUser;
    project: IProject;
}

export default ({ user, project }: IProjectParticipatingProps) => {
    const [ showSendProject, setShowSendProject ] = useState(false);
    const formik = useFormik({
        initialValues: {
            subject: "",
            mail: ""
        },
        onSubmit: (values) => {
            
        }
    });
    return <Card>
        <FormikContext.Provider value={formik}>
            <Form onSubmit={formik.handleSubmit}>
                <Dialog open={showSendProject} onClose={() => setShowSendProject(false)}>
                    <DialogContent>
                        <h3>Send project e-mail for '{project.name}'</h3>
                        <p>Hi {user.fullName ?? "project starter"}!</p>
                        <p>
                            You can send 4 e-mails in total (during and up to 180 days after the project ends). 
                            You have already sent { project.numberProjectEmailSent } e-mails, and can still send { 4 - project.numberProjectEmailSent } emails up until { new Date(project.end.getTime() + (1000 * 60 * 60 * 24 * 180)) } in the UTC (London) timezone. 
                            To personalize the message, you can add {'{firstname}'} and {'{lastname}'} to your message (including the brackets) - these will be substituted with the user's first and last name. 
                        </p>

                    </DialogContent>
                    <DialogActions>
                        <Button>Send E-Mail</Button>
                    </DialogActions>
                </Dialog>
            </Form>
        </FormikContext.Provider>
        <CardContent>
            <ProjectCard project={project} />
        </CardContent>
        <CardActions>
            <Button onClick={() => setShowSendProject(true)}>Send Project E-Mail</Button>
        </CardActions>
    </Card>;
}