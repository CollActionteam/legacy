import React, { useState, useCallback } from "react";
import { Paper, TableContainer, Table, TableHead, TableCell, TableRow, TableBody, Button, TablePagination, Checkbox, Dialog, DialogTitle, DialogContent, DialogActions, Card, TextField, makeStyles, FormGroup, FormControl, InputLabel, Select, MenuItem } from "@material-ui/core";
import { gql, useQuery, useMutation } from "@apollo/client";
import Loader from "../../Loader";
import { IProject } from "../../../api/types";
import { useHistory } from "react-router-dom";
import { Alert } from "../../Alert";
import { Form, useFormik, FormikProvider } from "formik";
import * as Yup from "yup";
import { Fragments } from "../../../api/fragments";
import { useDropzone } from "react-dropzone";
import Utils from "../../../utils";

interface IEditProjectProps {
    projectId: string;
}

const editProjectStyles = makeStyles(theme => ({
  root: {
    '& .MuiTextField-root': {
      margin: theme.spacing(1),
      width: '25ch'
    },
  },
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120,
  },
  selectEmpty: {
    marginTop: theme.spacing(2),
  },
}));

export const EditProject = ({ projectId } : IEditProjectProps): any => {
    const classes = editProjectStyles();
    const history = useHistory();
    const [ error, setError ] = useState<string | null>(null);
    const [ bannerImage, setBannerImage ] = useState<File | null>(null);
    const [ descriptiveImage, setDescriptiveImage ] = useState<File | null>(null);
    const { data, loading} = useQuery(
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
            onError: (data) => setError(data.message),
            onCompleted: (data) => {
                if (data.project.updateProject.succeeded) {
                    history.push("/admin/projects/list");
                } else {
                    setError("Error updating project: " + data.project.updateProject.errors.map((e: any) => e.errorMessage).join(", "));
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
            ownerId: -1,
            goal: "",
            status: "",
            proposal: "",
            target: "",
            anonymousUserParticipants: 0,
            firstCategory: "",
            secondCategory: "",
            tags: "",
            numberProjectEmailsSend: 0,
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
                                    .when('ownerId', (ownerId: number, schema: any) => ownerId === -1 ? schema.oneOf([""], "User with e-mail does not exist") : schema),
            ownerId: Yup.number(),
            goal: Yup.string(),
            status: Yup.string(),
            proposal: Yup.string(),
            target: Yup.number().required("Must be filled in").min(1, "Must be a positive number"),
            anonymousUserParticipants: Yup.number().required("Must be filled in").min(0, "Must be a positive number"),
            firstCategory: Yup.string(),
            secondCategory: Yup.string(),
            tags: Yup.string(),
            numberProjectEmailsSend: Yup.number().required("Must be filled in").min(0, "Must be a positive number"),
            bannerImageDescription: Yup.string(),
            descriptiveImageDescription: Yup.string(),
            initialized: Yup.boolean().oneOf([true], "Data must be loaded")
        }),
        onSubmit: async (values: any) => {
            if (values.ownerEmail !== "" && values.ownerId === -1) {
                setError(`Owner ${values.ownerEmail} could not be found`);
                return;
            }

            const uploads = { banner: null, descriptive: null };
            if (bannerImage !== null) {
                uploads.banner = await Utils.uploadImage(bannerImage, values.bannerImageDescription);
            }
            if (descriptiveImage !== null) {
                uploads.descriptive = await Utils.uploadImage(descriptiveImage, values.descriptiveImageDescription);
            }

            updateProject({
                variables: {
                    project: {
                        id: data.project.id,
                        name: values.name,
                        categories: [values.firstCategory, values.secondCategory].filter(c => c !== "NONE"),
                        target: values.target,
                        proposal: values.proposal,
                        description: values.description,
                        goal: values.goal,
                        creatorComments: values.creatorComments,
                        start: values.start,
                        end: values.end,
                        descriptionVideoLink: values.descriptionVideoLink,
                        displayPriority: values.displayPriority,
                        numberProjectEmailsSend: values.numberProjectEmailsSend,
                        status: values.status,
                        tags: values.tags.split(';').filter((i: string | null) => i),
                        ownerId: values.ownerId ?? -1,
                        bannerImageFileId: uploads.banner ?? data.project.bannerImage?.Id,
                        descriptiveImageFileId: uploads.descriptive ?? data.project.descriptiveImage?.Id
                    }
                }
            });
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
            ownerEmail: data.project.owner?.email ?? "",
            ownerId: formik.values.ownerId,
            status: data.project.status,
            anonymousUserParticipants: data.project.anonymousUserParticipants,
            firstCategory: data.project.categories[0]?.category ?? "NONE",
            secondCategory: data.project.categories[1]?.category ?? "NONE",
            tags: data.project.tags.map((t: any) => t.tag.name).join(";"),
            numberProjectEmailsSend: data.project.numberProjectEmailsSend,
            bannerImageDescription: data.project.bannerImage?.description,
            descriptiveImageDescription: data.project.descriptiveImage?.description
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
    if (ownerData && formik.values.ownerId !== ownerData.user.id) {
        formik.setFieldValue("ownerId", ownerData.user.id);
    }
    if (ownerError && formik.values.ownerId !== -1) {
        formik.setFieldValue("ownerId", -1);
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
            <Form className={classes.root} onSubmit={formik.handleSubmit}>
                <FormGroup>
                    <FormControl>
                        <TextField
                            name="name"
                            label="Project Name"
                            type="text"
                            { ...formik.getFieldProps('name') }
                        />
                        { formik.touched.name ? <Alert type="error" text={formik.errors.name} /> : null }
                    </FormControl>
                    <FormControl>
                        <TextField
                            name="description"
                            label="Description"
                            type="text"
                            { ...formik.getFieldProps('description') }
                        />
                        { formik.touched.description ? <Alert type="error" text={formik.errors.description} /> : null }
                    </FormControl>
                    <FormControl>
                        <TextField
                            name="proposal"
                            label="Project Proposal"
                            type="text"
                            { ...formik.getFieldProps('proposal') }
                        />
                        { formik.touched.proposal ? <Alert type="error" text={formik.errors.proposal} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="goal"
                            label="Project Goal"
                            type="text"
                            { ...formik.getFieldProps('goal') }
                        />
                        { formik.touched.goal ? <Alert type="error" text={formik.errors.goal} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="creatorComments"
                            label="Creator Comments"
                            type="text"
                            { ...formik.getFieldProps('creatorComments') }
                        />
                        { formik.touched.creatorComments ? <Alert type="error" text={formik.errors.creatorComments} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="target"
                            label="Target"
                            type="number"
                            { ...formik.getFieldProps('target') }
                        />
                        { formik.touched.target ? <Alert type="error" text={formik.errors.target} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="start"
                            label="Start"
                            type="date"
                            InputLabelProps={{ shrink: true }}
                            { ...formik.getFieldProps('start') }
                        />
                        { formik.touched.start ? <Alert type="error" text={formik.errors.start} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="end"
                            label="End"
                            type="date"
                            InputLabelProps={{ shrink: true }}
                            { ...formik.getFieldProps('end') }
                        />
                        { formik.touched.end ? <Alert type="error" text={formik.errors.end} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="descriptionVideoLink"
                            label="Description Video Link"
                            type="text"
                            { ...formik.getFieldProps('descriptionVideoLink') }
                        />
                        { formik.touched.descriptionVideoLink ? <Alert type="error" text={formik.errors.descriptionVideoLink} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="ownerEmail"
                            label="Owner E-Mail"
                            type="text"
                            { ...formik.getFieldProps('ownerEmail') }
                        />
                        { formik.touched.ownerEmail ? <Alert type="error" text={formik.errors.ownerEmail} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="tags"
                            label="Tags - separated by ';'"
                            type="text"
                            { ...formik.getFieldProps('tags') }
                        />
                        { formik.touched.tags ? <Alert type="error" text={formik.errors.tags} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="numberProjectEmailsSend"
                            label="Number Project E-Mails Send"
                            type="number"
                            { ...formik.getFieldProps('numberProjectEmailsSend') }
                        />
                        { formik.touched.numberProjectEmailsSend ? <Alert type="error" text={formik.errors.numberProjectEmailsSend} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="anonymousUserParticipants"
                            label="Anonymous Participants"
                            type="number"
                            { ...formik.getFieldProps('anonymousUserParticipants') }
                        />
                        { formik.touched.anonymousUserParticipants ? <Alert type="error" text={formik.errors.anonymousUserParticipants} /> : null }
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="first-category-label">First Category</InputLabel>
                        <Select name="firstCategory" labelId="first-category-label" { ...formik.getFieldProps('firstCategory')}>
                            <MenuItem key="" value="NONE">NONE</MenuItem>
                            { data?.categories.enumValues.map((c: any) => <MenuItem key={c.name} value={c.name}>{c.name}</MenuItem>) }
                        </Select>
                        { formik.touched.firstCategory ? <Alert type="error" text={formik.errors.firstCategory} /> : null }
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="second-category-label">Second Category</InputLabel>
                        <Select labelId="second-category-label" name="secondCategory" { ...formik.getFieldProps('secondCategory')}>
                            <MenuItem key="" value="NONE">NONE</MenuItem>
                            { data?.categories.enumValues.map((c: any) => <MenuItem key={c.name} value={c.name}>{c.name}</MenuItem>) }
                        </Select>
                        { formik.touched.secondCategory ? <Alert type="error" text={formik.errors.secondCategory} /> : null }
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="status-label">Status</InputLabel>
                        <Select labelId="status-label" name="status" { ...formik.getFieldProps('status')}>
                            { data?.statusses.enumValues.map((c: any) => <MenuItem key={c.name} value={c.name}>{c.name}</MenuItem>) }
                        </Select>
                        { formik.touched.status ? <Alert type="error" text={formik.errors.status} /> : null }
                    </FormControl>
                    <FormControl className={classes.formControl}>
                        <InputLabel shrink id="display-priority-label">Display Priority</InputLabel>
                        <Select labelId="display-priority-label" name="displayPriority" { ...formik.getFieldProps('displayPriority')}>
                            { data?.displayPriorities.enumValues.map((c: any) => <MenuItem key={c.name} value={c.name}>{c.name}</MenuItem>) }
                        </Select>
                        { formik.touched.displayPriority ? <Alert type="error" text={formik.errors.displayPriority} /> : null }
                    </FormControl>
                    <FormControl >
                        <TextField
                            name="descriptiveImageDescription"
                            label="Descriptive Image Description"
                            InputLabelProps={{ shrink: true }}
                            type="text"
                            { ...formik.getFieldProps('descriptiveImageDescription') }
                        />
                        { formik.touched.descriptiveImageDescription ? <Alert type="error" text={formik.errors.descriptiveImageDescription} /> : null }
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
                        { formik.touched.bannerImageDescription ? <Alert type="error" text={formik.errors.bannerImageDescription} /> : null }
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
}

export const ManageProjects = () => {
    const history = useHistory();
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [toDelete, setToDelete] = useState<IProject | null>(null);
    const [info, setInfo] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const {data, loading} = useQuery(
        GET_PROJECTS,
        {
            fetchPolicy: "cache-and-network", // To ensure it updates after deleting/editting
            variables: {
                skip: rowsPerPage * page,
                take: rowsPerPage,
                orderBy: "name"
            }
        }
    );
    const [ deleteProject ] = useMutation(
        DELETE_PROJECT,
        {
            variables: {
                id: toDelete?.id
            },
            onCompleted: (_data) => {
                setDeleteDialogOpen(false);
                setError(null);
            },
            onError: (data) => {
                setDeleteDialogOpen(false);
                setInfo(null);
                setError(data.message);
            },
            awaitRefetchQueries: true,
            refetchQueries: [{ 
                query: GET_PROJECTS,
                variables: {
                    skip: rowsPerPage * page,
                    take: rowsPerPage,
                    orderBy: "name"
                }
            }]
        }
    );
    const projectCount = data?.projectCount ?? 0;

    return <React.Fragment>
        { loading ? <Loader /> : null }
        <Alert type="info" text={info} />
        <Alert type="error" text={error} />
        <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
            <DialogTitle>Delete project { toDelete?.name }?</DialogTitle>
            <DialogContent>
                Are you sure you wish to delete "{ toDelete?.name }"?
            </DialogContent>
            <DialogActions>
                <Button onClick={() => deleteProject()}>Yes</Button>
                <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
            </DialogActions>
        </Dialog>
        <TableContainer component={Paper}>
            <Table aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell><h4>Name</h4></TableCell>
                        <TableCell align="right"><h4>Status</h4></TableCell>
                        <TableCell align="right"><h4>Start</h4></TableCell>
                        <TableCell align="right"><h4>End</h4></TableCell>
                        <TableCell align="right"><h4>Active</h4></TableCell>
                        <TableCell align="right"><h4>Edit</h4></TableCell>
                        <TableCell align="right"><h4>Delete</h4></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    { data?.projects.map((p: IProject) => (
                        <TableRow key={p.id}>
                            <TableCell component="th" scope="row">{ p.name }</TableCell>
                            <TableCell align="right">{ p.status }</TableCell>
                            <TableCell align="right">{ p.start }</TableCell>
                            <TableCell align="right">{ p.end }</TableCell>
                            <TableCell align="right"><Checkbox readOnly checked={ p.isActive } /></TableCell>
                            <TableCell align="right"><Button onClick={() => history.push(`/admin/projects/edit/${p.id}`)}>Edit</Button></TableCell>
                            <TableCell align="right"><Button onClick={() => { setDeleteDialogOpen(true); setToDelete(p); }}>Delete</Button></TableCell>
                        </TableRow>))
                    }
                    <TableRow>
                        <TablePagination count={projectCount} page={page} rowsPerPageOptions={[5, 10, 25, 50]} rowsPerPage={rowsPerPage} onChangePage={(_ev, newPage) => setPage(newPage)} onChangeRowsPerPage={(ev) => { setPage(0); setRowsPerPage(parseInt((ev.target.value))) }} />
                    </TableRow>
                </TableBody>
            </Table>
        </TableContainer>
    </React.Fragment>;
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
            numberProjectEmailsSend
            anonymousUserParticipants
            ${Fragments.projectDetail}
        }
        categories: __type(name: "Category") {
            enumValues {
                name
            }
        }
        displayPriorities: __type(name: "ProjectDisplayPriority") {
            enumValues {
                name
            }
        }
        statusses: __type(name: "ProjectStatus") {
            enumValues {
                name
            }
        }
    }
`;

const GET_PROJECTS = gql`
    query GetProjectData($skip: Int!, $take: Int!, $orderBy: String!) {
        projects(orderBy: [{ path: $orderBy, descending: false}], skip: $skip, take: $take) {
            ${Fragments.projectDetail}
        }
        projectCount
    }
`;

const DELETE_PROJECT = gql`
    mutation DeleteProject($id: ID!) {
        project {
            deleteProject(id: $id)
        }
    }
`;