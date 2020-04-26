import React, { useState, useCallback } from "react";
import { useHistory } from "react-router-dom";
import { Card, TextField, makeStyles, FormGroup, FormControl, InputLabel, Select, MenuItem } from "@material-ui/core";
import { gql, useQuery, useMutation } from "@apollo/client";
import { Form, useFormik, FormikProvider } from "formik";
import { useDropzone } from "react-dropzone";
import * as Yup from "yup";
import { Fragments } from "../../../api/fragments";
import Utils from "../../../utils";

import Loader from "../../Loader/Loader";
import { Alert } from "../../Alert/Alert";
import { Button } from "../../Button/Button";
import { useSettings } from "../../../providers/SettingsProvider";

interface IEditProjectProps {
    projectId: string;
}

const editProjectStyles = makeStyles(theme => ({
  root: {
    '& .MuiTextField-root': {
      margin: theme.spacing(1)
    },
  },
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120,
  }
}));

export default ({ projectId } : IEditProjectProps): any => {
    const classes = editProjectStyles();
    const history = useHistory();
    const { categories, displayPriorities, projectStatusses } = useSettings();
    const [ error, setError ] = useState<string | null>(null);
    const [ bannerImage, setBannerImage ] = useState<File | null>(null);
    const [ descriptiveImage, setDescriptiveImage ] = useState<File | null>(null);
    const { data, loading, error: loadingError } = useQuery(
        GET_PROJECT,
        {
            variables: {
                id: projectId
            }
        }
    );
    const [ updateProject ] = useMutation(
        UDPATE_PROJECT,
        {
            onError: (data) => {
                console.error(data.message);
                setError(data.message) 
            },
            onCompleted: (data) => {
                if (data.project.updateProject.succeeded) {
                    history.push("/admin/projects/list");
                } else {
                    const err = "Error updating project: " + data.project.updateProject.errors.map((e: any) => e.errorMessage).join(", ");
                    console.error(err);
                    setError(err);
                }
            }
        });
    const formik = useFormik({
        initialValues: {
            initialized: false,
            name: "",
            description: "",
            creatorComments: "",
            descriptionVideoLink: "",
            displayPriority: "",
            start: "",
            end: "",
            ownerEmail: "",
            ownerId: "",
            goal: "",
            status: "",
            proposal: "",
            target: "",
            anonymousUserParticipants: 0,
            firstCategory: "",
            secondCategory: "",
            tags: "",
            numberProjectEmailsSent: 0,
            bannerImageDescription: "",
            descriptiveImageDescription: "",
        },
        validationSchema: Yup.object({
            name: Yup.string().required(),
            description: Yup.string(),
            creatorComments: Yup.string(),
            descriptionVideoLink: Yup.string(),
            displayPriority: Yup.string(),
            start: Yup.date(),
            end: Yup.date(),
            ownerEmail: Yup.string().email("Please enter a valid e-mail address")
                                    .when('ownerId', (ownerId: string, schema: any) => ownerId === "" ? schema.oneOf([""], "User with e-mail does not exist") : schema),
            ownerId: Yup.string(),
            goal: Yup.string(),
            status: Yup.string(),
            proposal: Yup.string(),
            target: Yup.number().required("Must be filled in").min(1, "Must be a positive number"),
            anonymousUserParticipants: Yup.number().required("Must be filled in").min(0, "Must be a positive number"),
            firstCategory: Yup.string(),
            secondCategory: Yup.string(),
            tags: Yup.string(),
            numberProjectEmailsSent: Yup.number().required("Must be filled in").min(0, "Must be a positive number"),
            bannerImageDescription: Yup.string(),
            descriptiveImageDescription: Yup.string(),
            initialized: Yup.boolean().oneOf([true], "Data must be loaded")
        }),
        onSubmit: async (values: any) => {
            const uploads = { banner: null, descriptive: null, card: null };
            if (bannerImage !== null) {
                uploads.banner = await Utils.uploadImage(bannerImage, values.bannerImageDescription, 1600);
                uploads.card = await Utils.uploadImage(bannerImage, values.bannerImageDescription, 370);
            }
            if (descriptiveImage !== null) {
                uploads.descriptive = await Utils.uploadImage(descriptiveImage, values.descriptiveImageDescription, 1600);
            }

            await updateProject({
                variables: {
                    project: {
                        id: data.project.id,
                        name: values.name,
                        categories: [values.firstCategory, values.secondCategory].filter(c => c !== "NONE"),
                        target: values.target,
                        proposal: values.proposal,
                        description: values.description,
                        goal: values.goal,
                        creatorComments: values.creatorComments === "" ? null : values.creatorComments,
                        start: values.start,
                        end: values.end,
                        descriptionVideoLink: values.descriptionVideoLink === "" ? null : values.descriptionVideoLink,
                        displayPriority: values.displayPriority,
                        numberProjectEmailsSent: values.numberProjectEmailsSent,
                        status: values.status,
                        tags: values.tags.split(';').filter((i: string | null) => i),
                        ownerId: values.ownerId === "" ? null : values.ownerId,
                        bannerImageFileId: uploads.banner ?? data.project.bannerImage?.Id,
                        cardImageFileId: uploads.card ?? data.project.cardImage?.id,
                        descriptiveImageFileId: uploads.descriptive ?? data.project.descriptiveImage?.Id
                    }
                }
            });
            formik.setSubmitting(false);
        }
    });
    if (data && !formik.values.initialized) {
        formik.setValues({
            initialized: true,
            name: data.project.name,
            proposal: data.project.proposal,
            description: data.project.description,
            goal: data.project.goal,
            creatorComments: data.project.creatorComments ?? "",
            target: data.project.target,
            descriptionVideoLink: data.project.descriptionVideoLink ?? "",
            displayPriority: data.project.displayPriority,
            start: data.project.start,
            end: data.project.end,
            ownerEmail: data.project.ownerWithEmail?.email ?? "",
            ownerId: formik.values.ownerId,
            status: data.project.status,
            anonymousUserParticipants: data.project.anonymousUserParticipants,
            firstCategory: data.project.categories[0]?.category ?? "NONE",
            secondCategory: data.project.categories[1]?.category ?? "NONE",
            tags: data.project.tags.map((t: any) => t.tag.name).join(";"),
            numberProjectEmailsSent: data.project.numberProjectEmailsSent,
            bannerImageDescription: data.project.bannerImage?.description ?? "",
            descriptiveImageDescription: data.project.descriptiveImage?.description ?? ""
        });
    }
    const { data: ownerData, error: ownerError } = useQuery(
        GET_OWNER,
        {
            variables: {
                email: formik.values.ownerEmail
            }
        }
    );
    if (ownerError && formik.values.ownerId !== "") {
        formik.setFieldValue("ownerId", "");
    } else if (!ownerError && ownerData && formik.values.ownerId !== ownerData.user.id) {
        formik.setFieldValue("ownerId", ownerData.user.id);
    }
    const onDropBanner = useCallback((acceptedFiles: File[]) => {
        const lastFile = acceptedFiles[acceptedFiles.length - 1];
        setBannerImage(lastFile);
    }, []);
    const onDropDescriptive = useCallback((acceptedFiles: File[]) => {
        const lastFile = acceptedFiles[acceptedFiles.length - 1];
        setDescriptiveImage(lastFile);
    }, []);
    const { getRootProps: getBannerImageRootProps, getInputProps: getBannerImageInputProps, isDragActive: isBannerImageDragActive } = useDropzone({ onDrop: onDropBanner, accept: 'image/jpeg, image/png, image/gif, image/bmp' });
    const { getRootProps: getDescriptiveImageRootProps, getInputProps: getDescriptiveImageInputProps, isDragActive: isDescriptiveImageDragActive } = useDropzone({ onDrop: onDropDescriptive, accept: 'image/jpeg, image/png, image/gif, image/bmp' });
    return <FormikProvider value={formik}>
        { loading ? <Loader /> : null }
        <Card>
            <Alert type="error" text={error} />
            <Alert type="error" text={loadingError?.message} />
            <Form className={classes.root} onSubmit={formik.handleSubmit}>
                <FormGroup>
                    <FormControl>
                        <TextField
                            name="name"
                            label="Project Name"
                            type="text"
                            { ...formik.getFieldProps('name') }
                        />
                        <Alert type="error" text={formik.errors.name} />
                    </FormControl>
                    <FormControl>
                        <TextField
                            name="description"
                            label="Description"
                            type="text"
                            multiline={true}
                            rows={5}
                            { ...formik.getFieldProps('description') }
                        />
                        <Alert type="error" text={formik.errors.description} />
                    </FormControl>
                    <FormControl>
                        <TextField
                            name="proposal"
                            label="Project Proposal"
                            type="text"
                            multiline={true}
                            rows={5}
                            { ...formik.getFieldProps('proposal') }
                        />
                        <Alert type="error" text={formik.errors.proposal} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="goal"
                            label="Project Goal"
                            type="text"
                            multiline={true}
                            rows={5}
                            { ...formik.getFieldProps('goal') }
                        />
                        <Alert type="error" text={formik.errors.goal} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="creatorComments"
                            multiline={true}
                            rows={5}
                            label="Creator Comments"
                            type="text"
                            { ...formik.getFieldProps('creatorComments') }
                        />
                        <Alert type="error" text={formik.errors.creatorComments} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="target"
                            label="Target"
                            type="number"
                            { ...formik.getFieldProps('target') }
                        />
                        <Alert type="error" text={formik.errors.target} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="start"
                            label="Start"
                            type="date"
                            InputLabelProps={{ shrink: true }}
                            { ...formik.getFieldProps('start') }
                        />
                        <Alert type="error" text={formik.errors.start} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="end"
                            label="End"
                            type="date"
                            InputLabelProps={{ shrink: true }}
                            { ...formik.getFieldProps('end') }
                        />
                        <Alert type="error" text={formik.errors.end} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="descriptionVideoLink"
                            label="Description Video Link"
                            type="text"
                            { ...formik.getFieldProps('descriptionVideoLink') }
                        />
                        <Alert type="error" text={formik.errors.descriptionVideoLink} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="ownerEmail"
                            label="Owner E-Mail"
                            type="text"
                            { ...formik.getFieldProps('ownerEmail') }
                        />
                        <Alert type="error" text={formik.errors.ownerEmail} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="tags"
                            label="Tags - separated by ';'"
                            type="text"
                            { ...formik.getFieldProps('tags') }
                        />
                        <Alert type="error" text={formik.errors.tags} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="numberProjectEmailsSent"
                            label="Number Project E-Mails Send"
                            type="number"
                            { ...formik.getFieldProps('numberProjectEmailsSent') }
                        />
                        <Alert type="error" text={formik.errors.numberProjectEmailsSent} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="anonymousUserParticipants"
                            label="Anonymous Participants"
                            type="number"
                            { ...formik.getFieldProps('anonymousUserParticipants') }
                        />
                        <Alert type="error" text={formik.errors.anonymousUserParticipants} />
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="first-category-label">First Category</InputLabel>
                        <Select name="firstCategory" labelId="first-category-label" { ...formik.getFieldProps('firstCategory')}>
                            <MenuItem key="" value="NONE">NONE</MenuItem>
                            { categories.map(c => <MenuItem key={c} value={c}>{c}</MenuItem>) }
                        </Select>
                        <Alert type="error" text={formik.errors.firstCategory} />
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="second-category-label">Second Category</InputLabel>
                        <Select labelId="second-category-label" name="secondCategory" { ...formik.getFieldProps('secondCategory')}>
                            <MenuItem key="" value="NONE">NONE</MenuItem>
                            { categories.map(c => <MenuItem key={c} value={c}>{c}</MenuItem>) }
                        </Select>
                        <Alert type="error" text={formik.errors.secondCategory} />
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="status-label">Status</InputLabel>
                        <Select labelId="status-label" name="status" { ...formik.getFieldProps('status')}>
                            { projectStatusses.map(s => <MenuItem key={s} value={s}>{s}</MenuItem>) }
                        </Select>
                        <Alert type="error" text={formik.errors.status} />
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="display-priority-label">Display Priority</InputLabel>
                        <Select labelId="display-priority-label" name="displayPriority" { ...formik.getFieldProps('displayPriority')}>
                            { displayPriorities.map(d => <MenuItem key={d} value={d}>{d}</MenuItem>) }
                        </Select>
                        <Alert type="error" text={formik.errors.displayPriority} />
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="descriptiveImageDescription"
                            label="Descriptive Image Description"
                            InputLabelProps={{ shrink: true }}
                            type="text"
                            { ...formik.getFieldProps('descriptiveImageDescription') }
                        />
                        <Alert type="error" text={formik.errors.descriptiveImageDescription} />
                    <FormControl className={classes.formControl}>
                        <div {...getDescriptiveImageRootProps()}>
                            <input {...getDescriptiveImageInputProps()} />
                            {
                                descriptiveImage ?
                                    <p>Image uploaded: { descriptiveImage.name }</p> :
                                    isDescriptiveImageDragActive ?
                                        <p>Drop the descriptive image here...</p> :
                                        <p>Drag 'n' drop the descriptive image here, or click to select the image</p>
                            }
                        </div>
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        { data?.project?.descriptiveImage?.url ?
                            <FormControl className={classes.formControl}>
                                <h4>Current Descriptive Image</h4>
                                <img src={data.project.descriptiveImage.url} alt={data.project.descriptiveImage.description} width={300} height={300} />
                            </FormControl>
                            : null
                        }
                    </FormControl>
                    <FormControl ></FormControl>
                        <TextField
                            name="bannerImageDescription"
                            label="Banner Image Description"
                            type="text"
                            InputLabelProps={{ shrink: true }}
                            { ...formik.getFieldProps('bannerImageDescription') }
                        />
                        <Alert type="error" text={formik.errors.bannerImageDescription} />
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <div {...getBannerImageRootProps()}>
                            <input {...getBannerImageInputProps()} />
                            {
                                bannerImage ?
                                    <p>Image uploaded: { bannerImage.name }</p> :
                                    isBannerImageDragActive ?
                                        <p>Drop the banner image here...</p> :
                                        <p>Drag 'n' drop the banner image here, or click to select the image</p>
                            }
                        </div>
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        { data?.project?.bannerImage?.url ?
                            <FormControl className={classes.formControl}>
                                <h4>Current Banner Image</h4>
                                <img src={data.project.bannerImage.url} alt={data.project.bannerImage.description} width={300} height={300} />
                            </FormControl>
                            : null
                        }
                    </FormControl>
                    <Button type="submit" disabled={formik.isSubmitting}>Submit</Button>
                </FormGroup>
            </Form>
        </Card>
    </FormikProvider>;
};

const UDPATE_PROJECT = gql`
    mutation UpdateProject($project: UpdatedProjectInputGraph!) {
        project {
            updateProject(project: $project) {
                succeeded
                errors {
                    errorMessage
                    memberNames
                }
                project {
                    ${Fragments.projectDetail}
                }
            }
        }
    }
`;

const GET_OWNER = gql`
    query GetOwner($email: String) {
        user(where: [{path: "email", comparison: equal, value: [$email]}]) {
            id
        }
    }`;

const GET_PROJECT = gql`
    query GetProject($id: ID!) {
        project(id: $id) {
            numberProjectEmailsSent
            anonymousUserParticipants
            ownerWithEmail: owner {
                email
            }
            ${Fragments.projectDetail}
        }
    }
`;