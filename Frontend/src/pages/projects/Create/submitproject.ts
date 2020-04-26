import { ExecutionResult, gql, useMutation } from "@apollo/client";
import { IProjectResult } from "../../../api/types";
import { IProjectForm } from "./form";
import Utils from "../../../utils";

async function useSubmitProject(form: IProjectForm): Promise<IProjectResult> {
  let bannerId;
  if (form.banner) {
    bannerId = await Utils.uploadImage(form.banner, form.projectName, 1600);
  }

  let imageId;
  if (form.image) {
    imageId = await Utils.uploadImage(form.image, form.imageDescription, 1600);
  }

  let cardId;
  if (form.banner) {
    cardId = await Utils.uploadImage(form.banner, form.projectName, 370);
  }

  const [createProject] = useMutation(gql`
    mutation Create($project: NewProjectInputGraph) {
      project {
        createProject(project: $project) {
          succeeded
          errors {
            errorMessage
            memberNames
          }
        }
      }
    }
  `);

  const response = (await createProject({
    variables: {
      project: {
        name: form.projectName,
        bannerImageFileId: bannerId || null,
        categories: [form.category],
        target: form.target,
        proposal: form.proposal,
        description: form.description,
        start: form.startDate,
        end: form.endDate,
        goal: form.goal,
        tags: form.tags ? form.tags.split(';') : [],
        creatorComments: form.comments || null,
        descriptiveImageFileId: imageId || null,
        cardImageFileId: cardId || null,
        descriptionVideoLink: form.youtube || null
      }
    }
  })) as ExecutionResult;

  if (!response || !response.data) {
    throw new Error('No response from graphql endpoint');
  }

  return response.data.project.createProject;
}

export default useSubmitProject;