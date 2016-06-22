using System;
using System.Web.Mvc;
using BusinessLogic.Contracts;
using BusinessLogic.Impl;
using DataTransfer;
using DataTransfer.Enums;
using Microsoft.AspNet.Identity;

namespace TreeDemo.Controllers
{
    public class TreeController : Controller
    {
        private readonly IMyTree _tree;

        public TreeController()
        {
            _tree = new DefaultTree();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTree(CreateTreeDto data)
        {
            data.UserId= Guid.Parse(User.Identity.GetUserId());
            data.Id = Guid.NewGuid();

            _tree.CreateTree(data);

            ViewBag.NewTreeId = data.Id;
            return ViewTree();
        }

        [HttpGet]
        public ActionResult ViewTree()
        {
            try
            {
                Guid userId = Guid.Parse(User.Identity.GetUserId());
                NodeResponseDto treeNode = _tree.GetTreeFromDb(new GetTreeRequestDto
                {
                    UserId = userId
                });
                if (treeNode != null)
                {
                    GetKeyValuesResponseDto values = _tree.GetKeyValues(new GetKeyValuesRequestDto
                    {
                        UserId = userId
                    });
                    ViewBag.KeyValues = values;
                }

                if (treeNode==null)
                {
                    return View("Index");
                }

                ViewBag.Tree = treeNode;
                
                return View("ViewTree");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult AddNode(Guid nodeId)
        {
            ViewBag.ParentId = nodeId;
            return View();
        }

        [HttpGet]
        public ActionResult ManageNode(string nodeId)
        {
            try
            {
                if (string.IsNullOrEmpty(nodeId))
                    throw new Exception("Empty node id");
                    
                NodeResponseDto node = _tree.GetNode(new NodeRequestDto
                {
                    NodeId = Guid.Parse(nodeId),
                    UserId = Guid.Parse(User.Identity.GetUserId())
                });

                ViewBag.Node = node;

                return View("ManageNode",new ChangeDataRequestDto());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult AddNode(AddNodeRequestDto data)
        {
            try
            {
                if (data.DigitValue <= 0)
                    throw new Exception("Only positive digits are allowed, no negatives, zeros or letters");

                data.UserId = Guid.Parse(User.Identity.GetUserId());
                data.NodeId = Guid.NewGuid();

                if (_tree.AddNode(data))
                {
                    return ViewTree();
                }
                return View("NodeExists");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ChangeData(ChangeDataRequestDto data)
        {
            try
            {
                if(data.DigitValue <= 0)
                    throw new Exception("Only positive digits are allowed, no negatives, zeros or letters");

                data.UserId= Guid.Parse(User.Identity.GetUserId());
                if (_tree.UpdateNodeData(data))
                {
                    return ManageNode(data.NodeId.ToString());
                }
                return View("NodeExists");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RemoveNode(string nodeId)
        {
            try
            {
                if (string.IsNullOrEmpty(nodeId))
                    throw new Exception("Empty node id");
                RemoveNodeRequestDto data=new RemoveNodeRequestDto
                {
                    NodeId = Guid.Parse(nodeId),
                    UserId = Guid.Parse(User.Identity.GetUserId())
                };
                _tree.RemoveNode(data);
                return ViewTree();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult GetNodeList(int order=0)
        {
            if (order != 1 && order != 0 && order != 2)
            {
                order = 0;
            }
            OrderedValuesResponseDto result = _tree.OrderedValues(new OrderedValuesRequestDto
            {
                UserId = Guid.Parse(User.Identity.GetUserId()),
                OrderBy = (SortOrderBy)order
            });
            return View(result);
        }

        [HttpPost]
        public ActionResult StoreToXml()
        {
            _tree.StoreXml(new StoreXmlRequestDto
            {
                UserId = Guid.Parse(User.Identity.GetUserId())
            });
            return ViewTree();
        }

        [HttpPost]
        public ActionResult RestoreFromXml()
        {
            _tree.RestoreFromXml(new RestoreFromXmlRequestDto
            {
                UserId = Guid.Parse(User.Identity.GetUserId())
            });
            return ViewTree();
        }
    }
}